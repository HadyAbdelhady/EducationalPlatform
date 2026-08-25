using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Centers.Interfaces;
using Application.Features.Sheets.DTOs;
using Domain.Entities;
using Domain.enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Sheets.Queries.GetSubmittedAnswers
{
    public class GetSubmittedAnswersQueryHandler(
        IUnitOfWork unitOfWork,
        IBaseFilterRegistry<AnswersSheet> answersSheetFilterRegistry,
        IInstructorContentScopeService instructorContentScopeService)
        : IRequestHandler<GetSubmittedAnswersQuery, Result<PaginatedResult<SubmittedAnswersSheetDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IBaseFilterRegistry<AnswersSheet> _answersSheetFilterRegistry = answersSheetFilterRegistry;
        private readonly IInstructorContentScopeService _instructorContentScopeService = instructorContentScopeService;

        public async Task<Result<PaginatedResult<SubmittedAnswersSheetDto>>> Handle(
            GetSubmittedAnswersQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (request.TargetType is not (SheetTargetType.Course or SheetTargetType.Section))
                {
                    return Result<PaginatedResult<SubmittedAnswersSheetDto>>.FailureStatusCode(
                        "targetType must be Course or Section.",
                        ErrorType.BadRequest);
                }

                var targetExists = request.TargetType == SheetTargetType.Course
                    ? await _unitOfWork.Repository<Course>().AnyAsync(c => c.Id == request.TargetId, cancellationToken)
                    : await _unitOfWork.Repository<Section>().AnyAsync(s => s.Id == request.TargetId, cancellationToken);

                if (!targetExists)
                {
                    return Result<PaginatedResult<SubmittedAnswersSheetDto>>.FailureStatusCode(
                        request.TargetType == SheetTargetType.Course ? "Course not found." : "Section not found.",
                        ErrorType.NotFound);
                }

                var courseId = request.TargetType == SheetTargetType.Course ? request.TargetId : (Guid?)null;
                var sectionId = request.TargetType == SheetTargetType.Section ? request.TargetId : (Guid?)null;
                await _instructorContentScopeService.ResolveAsync(
                    request.InstructorId,
                    courseId,
                    sectionId,
                    cancellationToken);

                var skeleton = request.RequestSkeleton ?? new GetAllEntityRequestSkeleton();
                var instructorId = request.InstructorId;
                var targetId = request.TargetId;

                var submissionsQuery = _unitOfWork.Repository<AnswersSheet>()
                    .GetAll(cancellationToken)
                    .AsNoTracking()
                    .Where(a => a.QuestionsSheet.InstructorId == instructorId);

                submissionsQuery = request.TargetType == SheetTargetType.Course
                    ? submissionsQuery.Where(a =>
                        a.QuestionsSheet.CourseId == targetId ||
                        (a.QuestionsSheet.Section != null && a.QuestionsSheet.Section.CourseId == targetId) ||
                        (a.QuestionsSheet.Video != null && a.QuestionsSheet.Video.Section.CourseId == targetId))
                    : submissionsQuery.Where(a =>
                        a.QuestionsSheet.SectionId == targetId ||
                        (a.QuestionsSheet.Video != null && a.QuestionsSheet.Video.SectionId == targetId));

                submissionsQuery = submissionsQuery
                    .ApplyFilters(skeleton.Filters, _answersSheetFilterRegistry.Filters)
                    .ApplySort(skeleton.SortBy, skeleton.IsDescending, _answersSheetFilterRegistry.Sorts);

                var projected = submissionsQuery.Select(a => new SubmittedAnswersSheetDto
                {
                    AnswersSheetId = a.Id,
                    Name = a.Name,
                    SheetUrl = a.SheetUrl,
                    IsApproved = a.IsApproved,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    StudentId = a.StudentId,
                    StudentName = a.Student.User.FullName,
                    StudentProfilePicture = a.Student.User.PersonalPictureUrl ?? string.Empty,
                    QuestionsSheetId = a.QuestionsSheetId,
                    QuestionsSheetName = a.QuestionsSheet.Name,
                    QuestionsSheetUrl = a.QuestionsSheet.SheetUrl,
                    DueDate = a.QuestionsSheet.DueDate,
                    CourseId = a.QuestionsSheet.CourseId
                        ?? (a.QuestionsSheet.Section != null ? a.QuestionsSheet.Section.CourseId : (Guid?)null)
                        ?? (a.QuestionsSheet.Video != null ? a.QuestionsSheet.Video.Section.CourseId : (Guid?)null),
                    CourseName = a.QuestionsSheet.Course != null
                        ? a.QuestionsSheet.Course.Name
                        : a.QuestionsSheet.Section != null
                            ? a.QuestionsSheet.Section.Course!.Name
                            : a.QuestionsSheet.Video != null
                                ? a.QuestionsSheet.Video.Section.Course!.Name
                                : null,
                    SectionId = a.QuestionsSheet.SectionId
                        ?? (a.QuestionsSheet.Video != null ? a.QuestionsSheet.Video.SectionId : (Guid?)null),
                    SectionName = a.QuestionsSheet.Section != null
                        ? a.QuestionsSheet.Section.Name
                        : a.QuestionsSheet.Video != null
                            ? a.QuestionsSheet.Video.Section.Name
                            : null,
                    VideoId = a.QuestionsSheet.VideoId,
                    VideoName = a.QuestionsSheet.Video != null ? a.QuestionsSheet.Video.Name : null
                });

                var pageSize = skeleton.PageSize <= 0 ? 10 : Math.Min(skeleton.PageSize, 25);
                var result = await projected.ToPaginatedResultAsync(skeleton.PageNumber, pageSize, cancellationToken);
                return Result<PaginatedResult<SubmittedAnswersSheetDto>>.Success(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Result<PaginatedResult<SubmittedAnswersSheetDto>>.FailureStatusCode(
                    ex.Message,
                    ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<SubmittedAnswersSheetDto>>.FailureStatusCode(
                    $"An error occurred while retrieving submitted answers: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
