using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Auth.Interfaces;
using Application.Features.Sheets.DTOs;
using Domain.Entities;
using Domain.enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Sheets.Queries.GetInstructorSheetsWithSubmissions
{
    public class GetInstructorSheetsWithSubmissionsQueryHandler(
        IUnitOfWork unitOfWork,
        IBaseFilterRegistry<Sheet> sheetFilterRegistry)
        : IRequestHandler<GetInstructorSheetsWithSubmissionsQuery, Result<PaginatedResult<QuestionSheetWithSubmissionsDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IBaseFilterRegistry<Sheet> _sheetFilterRegistry = sheetFilterRegistry;

        public async Task<Result<PaginatedResult<QuestionSheetWithSubmissionsDto>>> Handle(
            GetInstructorSheetsWithSubmissionsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var instructorExists = await _unitOfWork.GetRepository<IUserRepository>()
                    .DoesInstructorExistAsync(request.InstructorId, cancellationToken);
                if (!instructorExists)
                {
                    return Result<PaginatedResult<QuestionSheetWithSubmissionsDto>>.FailureStatusCode(
                        "Instructor not found.",
                        ErrorType.Forbidden);
                }

                var skeleton = request.RequestSkeleton ?? new GetAllEntityRequestSkeleton();

                var sheetsQuery = _unitOfWork.Repository<Sheet>()
                    .GetAll(cancellationToken)
                    .AsNoTracking()
                    .Where(s => s.Type == SheetType.QuestionSheet && s.InstructorId == request.InstructorId)
                    .ApplyFilters(skeleton.Filters, _sheetFilterRegistry.Filters)
                    .ApplySort(skeleton.SortBy, skeleton.IsDescending, _sheetFilterRegistry.Sorts);

                var projected = sheetsQuery.Select(s => new QuestionSheetWithSubmissionsDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    SheetUrl = s.SheetUrl,
                    DueDate = s.DueDate,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    CourseId = s.CourseId
                        ?? (s.Section != null ? s.Section.CourseId : (Guid?)null)
                        ?? (s.Video != null ? s.Video.Section.CourseId : (Guid?)null),
                    CourseName = s.Course != null
                        ? s.Course.Name
                        : s.Section != null
                            ? s.Section.Course!.Name
                            : s.Video != null
                                ? s.Video.Section.Course!.Name
                                : null,
                    SectionId = s.SectionId
                        ?? (s.Video != null ? s.Video.SectionId : (Guid?)null),
                    SectionName = s.Section != null
                        ? s.Section.Name
                        : s.Video != null
                            ? s.Video.Section.Name
                            : null,
                    VideoId = s.VideoId,
                    VideoName = s.Video != null ? s.Video.Name : null,
                    SubmissionsCount = s.AnswersSheets.Count,
                    Submissions = s.AnswersSheets
                        .OrderByDescending(a => a.CreatedAt)
                        .Select(a => new StudentAnswersSheetSubmissionDto
                        {
                            AnswersSheetId = a.Id,
                            Name = a.Name,
                            SheetUrl = a.SheetUrl,
                            IsApproved = a.IsApproved,
                            CreatedAt = a.CreatedAt,
                            UpdatedAt = a.UpdatedAt,
                            StudentId = a.StudentId,
                            StudentName = a.Student.User.FullName,
                            StudentProfilePicture = a.Student.User.PersonalPictureUrl ?? string.Empty
                        })
                        .ToList()
                });

                var pageSize = skeleton.PageSize <= 0 ? 10 : Math.Min(skeleton.PageSize, 25);
                var result = await projected.ToPaginatedResultAsync(skeleton.PageNumber, pageSize, cancellationToken);
                return Result<PaginatedResult<QuestionSheetWithSubmissionsDto>>.Success(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Result<PaginatedResult<QuestionSheetWithSubmissionsDto>>.FailureStatusCode(
                    ex.Message,
                    ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<QuestionSheetWithSubmissionsDto>>.FailureStatusCode(
                    $"An error occurred while retrieving instructor sheets: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
