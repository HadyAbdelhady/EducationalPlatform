using Application.DTOs.Sheets;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.AnswersSheets.Commands.CreateAnswersSheet
{
    public class CreateAnswersSheetCommandHandler(IUnitOfWork unitOfWork, ICloudinaryCore cloudinaryService)
        : IRequestHandler<CreateAnswersSheetCommand, Result<AnswersSheetCreationResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICloudinaryCore _cloudinaryService = cloudinaryService;

        public async Task<Result<AnswersSheetCreationResponse>> Handle(CreateAnswersSheetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate QuestionsSheet exists
                var questionsSheet = await _unitOfWork.Repository<Sheet>().GetByIdAsync(request.QuestionsSheetId, cancellationToken);
                if (questionsSheet is null)
                {
                    return Result<AnswersSheetCreationResponse>.FailureStatusCode("Questions sheet not found", ErrorType.NotFound);
                }

                if (questionsSheet.DueDate.HasValue && DateTimeOffset.UtcNow >= questionsSheet.DueDate.Value)
                {
                    return Result<AnswersSheetCreationResponse>.FailureStatusCode("The submission deadline has passed!",
                        ErrorType.BadRequest);
                }

                // Validate Student exists
                var student = await _unitOfWork.GetRepository<IUserRepository>().GetStudentByIdWithRelationsAsync(request.StudentId, cancellationToken);
                if (student is null)
                {
                    return Result<AnswersSheetCreationResponse>.FailureStatusCode("Student not found", ErrorType.NotFound);
                }

                // Upload PDF to Cloudinary
                var cloudinaryResult = await _cloudinaryService.UploadPdfAsync(request.SheetFile);

                var newAnswersSheet = new AnswersSheet()
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
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




