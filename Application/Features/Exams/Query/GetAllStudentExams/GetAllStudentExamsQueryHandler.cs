using Application.Features.Exams.DTOs;
using Application.Common;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Exams.Query.GetAllStudentExams
{
    public class GetAllStudentExamsQueryHandler(IUnitOfWork unitOfWork,
                                        IBaseFilterRegistry<Exam> examFilterRegistry) : IRequestHandler<GetAllExamsQuery, Result<PaginatedResult<ExamListDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IBaseFilterRegistry<Exam> _examFilterRegistry = examFilterRegistry;

        public async Task<Result<PaginatedResult<ExamListDto>>> Handle(
            GetAllExamsQuery request,
            CancellationToken cancellationToken)
        {
            var exams = _unitOfWork.Repository<Exam>()
                .GetAll(cancellationToken)
                .Where(e => e.Status != ExamStatus.Draft);

            if (request.UserId != Guid.Empty)
            {
                exams = exams.Where(e =>
                    e.Course!.StudentCourses.Any(sc => sc.StudentId == request.UserId) ||
                    (e.SectionId != null && e.Section!.StudentSections.Any(ss => ss.StudentId == request.UserId)));
            }

            var filters = new Dictionary<string, string>(request.RequestSkeleton.Filters, StringComparer.OrdinalIgnoreCase);

            if (filters.Remove("studentstatus", out var studentStatusStr) &&
                Enum.TryParse<ExamResultStatus>(studentStatusStr, true, out var studentStatus))
            {
                if (studentStatus == ExamResultStatus.NotStarted)
                {
                    exams = exams.Where(e => !e.ExamResults.Any(r =>
                        r.StudentId == request.UserId &&
                        (r.Status == ExamResultStatus.InProgress || r.Status == ExamResultStatus.Passed || r.Status == ExamResultStatus.Failed)));
                }
                else
                {
                    exams = exams.Where(e => e.ExamResults.Any(r =>
                        r.StudentId == request.UserId && r.Status == studentStatus));
                }
            }

            if (filters.Remove("istaken", out var isTakenStr) &&
                bool.TryParse(isTakenStr, out var isTaken))
            {
                if (isTaken)
                {
                    exams = exams.Where(e => e.ExamResults.Any(r =>
                        r.StudentId == request.UserId &&
                        (r.Status == ExamResultStatus.InProgress || r.Status == ExamResultStatus.Passed || r.Status == ExamResultStatus.Failed)));
                }
                else
                {
                    exams = exams.Where(e => !e.ExamResults.Any(r =>
                        r.StudentId == request.UserId &&
                        (r.Status == ExamResultStatus.InProgress || r.Status == ExamResultStatus.Passed || r.Status == ExamResultStatus.Failed)));
                }
            }

            // Remove studentid from incoming filters if present as enrollment is already applied
            filters.Remove("studentid");

            exams = exams
                .ApplyFilters(filters, _examFilterRegistry.Filters)
                .ApplySort(request.RequestSkeleton.SortBy, request.RequestSkeleton.IsDescending, _examFilterRegistry.Sorts);

            var now = EgyptTime.UtcNow;
            var examsQuery = exams.Select(e => new ExamListDto
            {
                ExamId = e.Id,
                Name = e.Name,
                Description = e.Description,
                ExamStatus = e.Status == ExamStatus.Draft
                    ? ExamStatus.Draft
                    : (e.Status == ExamStatus.Finished || (e.EndTime != null && e.EndTime <= now)
                        ? ExamStatus.Finished
                        : (e.StartTime != null && e.StartTime <= now
                            ? ExamStatus.Started
                            : ExamStatus.Scheduled)),
                StudentExamStatusResult = e.ExamResults
                    .Where(se => se.StudentId == request.UserId)
                    .Select(se => se.Status)
                    .FirstOrDefault(),
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                IsTaken = e.ExamResults.Any(se =>
                    se.StudentId == request.UserId &&
                    (se.Status == ExamResultStatus.InProgress || se.Status == ExamResultStatus.Passed || se.Status == ExamResultStatus.Failed)),
                TotalMark = e.TotalMark,
                NumberOfQuestions = e.NumberOfQuestions,
                DurationInMinutes = e.DurationInMinutes,
                IsRandomized = e.IsRandomized,
                ExamType = e.ExamType,
                PassMarkPercentage = e.PassMarkPercentage,
                ObtainedMarks = e.ExamResults
                    .Where(se => se.StudentId == request.UserId)
                    .Select(se => se.StudentMark)
                    .FirstOrDefault() ?? 0m,
                TakenAt = e.ExamResults
                    .Where(se => se.StudentId == request.UserId)
                    .Select(se => se.TakenAt)
                    .FirstOrDefault(),
            });

            var paginatedItems = await examsQuery.ToPaginatedResultAsync(
                request.RequestSkeleton.PageNumber,
                10,
                cancellationToken);

            return Result<PaginatedResult<ExamListDto>>.Success(paginatedItems);
        }
    }

}