using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Centers.Interfaces;
using Application.Features.Profiles.Interfaces;
using Application.Features.Sheets.DTOs;
using Domain.Entities;
using Domain.enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.HomeScreen.InstructorStudentSheets
{
    public class GetInstructorStudentSheetsQueryHandler(
        IUnitOfWork unitOfWork,
        IInstructorContentScopeService instructorContentScopeService)
        : IRequestHandler<GetInstructorStudentSheetsQuery, Result<PaginatedResult<SheetResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IInstructorContentScopeService _instructorContentScopeService = instructorContentScopeService;

        public async Task<Result<PaginatedResult<SheetResponse>>> Handle(
            GetInstructorStudentSheetsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var profileRepo = _unitOfWork.GetRepository<IProfileRepository>();
                if (!await profileRepo.StudentExistsAsync(request.StudentId, cancellationToken))
                {
                    return Result<PaginatedResult<SheetResponse>>.FailureStatusCode(
                        "Student not found.",
                        ErrorType.NotFound);
                }

                if (!await profileRepo.HasSharedContentAsync(
                        request.InstructorId,
                        request.StudentId,
                        cancellationToken))
                {
                    return Result<PaginatedResult<SheetResponse>>.FailureStatusCode(
                        "You are not eligible to view this student.",
                        ErrorType.Forbidden);
                }

                var scope = await _instructorContentScopeService.ResolveAsync(
                    request.InstructorId,
                    courseId: null,
                    sectionId: null,
                    cancellationToken);

                var studentId = request.StudentId;
                var courseIds = scope.CourseIds;
                var sectionIds = scope.SectionIds;

                var sheetsQuery = _unitOfWork.Repository<Sheet>()
                    .GetAll(cancellationToken)
                    .AsNoTracking()
                    .Where(s => s.Type == SheetType.QuestionSheet && s.InstructorId == request.InstructorId)
                    .Where(s =>
                        (s.CourseId.HasValue && courseIds.Contains(s.CourseId.Value)) ||
                        (s.SectionId.HasValue && sectionIds.Contains(s.SectionId.Value)) ||
                        (s.VideoId.HasValue && sectionIds.Contains(s.Video!.SectionId)) ||
                        (s.VideoId.HasValue && courseIds.Contains(s.Video!.Section!.CourseId)))
                    .OrderByDescending(s => s.DueDate ?? s.CreatedAt)
                    .Select(s => new SheetResponse
                    {
                        Id = s.Id,
                        Name = s.Name,
                        SheetUrl = s.AnswersSheets
                            .Where(a => a.StudentId == studentId)
                            .Select(a => a.SheetUrl)
                            .FirstOrDefault() ?? string.Empty,
                        CreatedAt = s.CreatedAt,
                        UpdatedAt = s.AnswersSheets
                            .Where(a => a.StudentId == studentId)
                            .Select(a => (DateTimeOffset?)a.CreatedAt)
                            .FirstOrDefault() ?? s.UpdatedAt,
                        DueDate = s.DueDate,
                        QuestionsSheetId = s.Id,
                        QuestionsSheetName = s.Name,
                        IsApproved = s.AnswersSheets
                            .Where(a => a.StudentId == studentId)
                            .Select(a => (bool?)a.IsApproved)
                            .FirstOrDefault()
                    });

                var page = request.Page < 1 ? 1 : request.Page;
                var pageSize = request.PageSize <= 0 ? 10 : Math.Min(request.PageSize, 25);
                var result = await sheetsQuery.ToPaginatedResultAsync(page, pageSize, cancellationToken);
                return Result<PaginatedResult<SheetResponse>>.Success(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Result<PaginatedResult<SheetResponse>>.FailureStatusCode(
                    ex.Message,
                    ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<SheetResponse>>.FailureStatusCode(
                    $"An error occurred while retrieving student sheets: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
