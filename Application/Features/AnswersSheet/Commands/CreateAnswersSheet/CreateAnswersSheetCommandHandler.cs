using Application.DTOs.Sheets;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.AnswersSheets.Commands.CreateAnswersSheet
{
    public class CreateAnswersSheetCommandHandler(
        IUnitOfWork unitOfWork,
        ICloudinaryCore cloudinaryService,
        IStudentEducationYearProvider studentEducationYearProvider)
        : IRequestHandler<CreateAnswersSheetCommand, Result<AnswersSheetCreationResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICloudinaryCore _cloudinaryService = cloudinaryService;
        private readonly IStudentEducationYearProvider _studentEducationYearProvider = studentEducationYearProvider;

        public async Task<Result<AnswersSheetCreationResponse>> Handle(CreateAnswersSheetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return Result<AnswersSheetCreationResponse>.FailureStatusCode("Name is required.", ErrorType.BadRequest);
                }

                if (request.SheetFile is null || request.SheetFile.Length == 0)
                {
                    return Result<AnswersSheetCreationResponse>.FailureStatusCode("A non-empty PDF file is required.", ErrorType.BadRequest);
                }

                var contentType = request.SheetFile.ContentType ?? string.Empty;
                var fileName = request.SheetFile.FileName ?? string.Empty;
                if (!contentType.Contains("pdf", StringComparison.OrdinalIgnoreCase) &&
                    !fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    return Result<AnswersSheetCreationResponse>.FailureStatusCode("Only PDF uploads are allowed.", ErrorType.BadRequest);
                }

                var questionsSheet = await _unitOfWork.Repository<Sheet>().GetByIdAsync(request.QuestionsSheetId, cancellationToken);
                if (questionsSheet is null)
                {
                    return Result<AnswersSheetCreationResponse>.FailureStatusCode("Questions sheet not found", ErrorType.NotFound);
                }

                if (questionsSheet.Type != SheetType.QuestionSheet)
                {
                    return Result<AnswersSheetCreationResponse>.FailureStatusCode(
                        "Submissions are only allowed for question sheets.",
                        ErrorType.BadRequest);
                }

                if (questionsSheet.DueDate.HasValue && DateTimeOffset.UtcNow >= questionsSheet.DueDate.Value)
                {
                    return Result<AnswersSheetCreationResponse>.FailureStatusCode("The submission deadline has passed!",
                        ErrorType.BadRequest);
                }

                var user = await _unitOfWork.GetRepository<IUserRepository>().GetStudentByIdWithRelationsAsync(request.StudentId, cancellationToken);
                if (user is null)
                {
                    return Result<AnswersSheetCreationResponse>.FailureStatusCode("Student not found", ErrorType.NotFound);
                }

                if (user.Student is null)
                {
                    return Result<AnswersSheetCreationResponse>.FailureStatusCode("User is not a student", ErrorType.BadRequest);
                }

                var duplicateExists = await _unitOfWork.Repository<AnswersSheet>().AnyAsync(
                    a => a.StudentId == request.StudentId && a.QuestionsSheetId == request.QuestionsSheetId,
                    cancellationToken);
                if (duplicateExists)
                {
                    return Result<AnswersSheetCreationResponse>.FailureStatusCode(
                        "You have already submitted for this sheet. Use the update endpoint to replace your submission.",
                        ErrorType.Conflict);
                }

                var enrollmentRepo = _unitOfWork.GetRepository<IStudentEnrollmentRepository>();
                Guid? owningCourseId = null;
                var hasAccess = false;

                if (questionsSheet.CourseId.HasValue)
                {
                    owningCourseId = questionsSheet.CourseId;
                    hasAccess = await enrollmentRepo.IsStudentEnrolledInCourseAsync(
                        request.StudentId,
                        questionsSheet.CourseId.Value,
                        cancellationToken);
                }
                else if (questionsSheet.SectionId.HasValue)
                {
                    var section = await _unitOfWork.Repository<Section>().GetByIdAsync(questionsSheet.SectionId.Value, cancellationToken);
                    if (section is null)
                    {
                        return Result<AnswersSheetCreationResponse>.FailureStatusCode(
                            "Questions sheet section not found.",
                            ErrorType.NotFound);
                    }

                    owningCourseId = section.CourseId;
                    hasAccess = await enrollmentRepo.CanStudentAccessSectionContentAsync(
                        request.StudentId,
                        section.Id,
                        cancellationToken);
                }
                else if (questionsSheet.VideoId.HasValue)
                {
                    var video = await _unitOfWork.Repository<Video>().GetByIdAsync(questionsSheet.VideoId.Value, cancellationToken);
                    if (video is null)
                    {
                        return Result<AnswersSheetCreationResponse>.FailureStatusCode(
                            "Questions sheet video not found.",
                            ErrorType.NotFound);
                    }

                    var section = await _unitOfWork.Repository<Section>().GetByIdAsync(video.SectionId, cancellationToken);
                    if (section is null)
                    {
                        return Result<AnswersSheetCreationResponse>.FailureStatusCode(
                            "Questions sheet section not found.",
                            ErrorType.NotFound);
                    }

                    owningCourseId = section.CourseId;
                    hasAccess = await enrollmentRepo.CanStudentAccessSectionContentAsync(
                        request.StudentId,
                        video.SectionId,
                        cancellationToken);
                }
                else
                {
                    return Result<AnswersSheetCreationResponse>.FailureStatusCode(
                        "Questions sheet is not associated with a course, section, or video.",
                        ErrorType.BadRequest);
                }

                if (!hasAccess)
                {
                    return Result<AnswersSheetCreationResponse>.FailureStatusCode(
                        "You are not enrolled in the content this sheet belongs to.",
                        ErrorType.BadRequest);
                }

                var course = await _unitOfWork.Repository<Course>().GetByIdAsync(owningCourseId!.Value, cancellationToken);
                if (course is null)
                {
                    return Result<AnswersSheetCreationResponse>.FailureStatusCode("Course not found.", ErrorType.NotFound);
                }

                var studentYearId = await _studentEducationYearProvider.GetEducationYearIdByUserIdAsync(request.StudentId, cancellationToken);
                if (!studentYearId.HasValue)
                {
                    return Result<AnswersSheetCreationResponse>.FailureStatusCode(
                        "Student has no education year assigned.",
                        ErrorType.BadRequest);
                }

                if (course.EducationYearId != studentYearId.Value)
                {
                    return Result<AnswersSheetCreationResponse>.FailureStatusCode(
                        "This sheet is not available for your education year.",
                        ErrorType.BadRequest);
                }

                var cloudinaryResult = await _cloudinaryService.UploadPdfAsync(request.SheetFile);

                var newAnswersSheet = new AnswersSheet()
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name.Trim(),
                    SheetUrl = cloudinaryResult.Url,
                    SheetPublicId = cloudinaryResult.PublicId,
                    QuestionsSheetId = request.QuestionsSheetId,
                    StudentId = request.StudentId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    IsDeleted = false
                };

                await _unitOfWork.Repository<AnswersSheet>().AddAsync(newAnswersSheet, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<AnswersSheetCreationResponse>.Success(new AnswersSheetCreationResponse
                {
                    AnswersSheetId = newAnswersSheet.Id,
                    SheetUrl = newAnswersSheet.SheetUrl,
                    CreatedAt = newAnswersSheet.CreatedAt
                });
            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<AnswersSheetCreationResponse>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<AnswersSheetCreationResponse>.FailureStatusCode($"Error creating answers sheet: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
