using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Exams.Common;
using Application.Features.Exams.DTOs;
using Domain.Entities;
using Domain.enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Exams.Query.GetAllExams
{
    public class GetAllExamsQueryHandler(IUnitOfWork unitOfWork,
                                        IBaseFilterRegistry<Exam> examFilterRegistry) : IRequestHandler<GetAllExamsQuery, Result<PaginatedResult<ExamListDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IBaseFilterRegistry<Exam> _examFilterRegistry = examFilterRegistry;

        public async Task<Result<PaginatedResult<ExamListDto>>> Handle(GetAllExamsQuery request, CancellationToken cancellationToken)
        {
            var isInstructor = request.UserId != Guid.Empty &&
                await _unitOfWork.Repository<User>()
                    .GetAll(cancellationToken)
                    .AnyAsync(u => u.Id == request.UserId && u.Instructor != null, cancellationToken);

            IQueryable<Exam> exams;

            if (isInstructor)
            {
                exams = _unitOfWork.Repository<Exam>()
                    .GetAll(cancellationToken)
                    .Where(e => e.InstructorId == request.UserId);
            }
            else
            {
                exams = _unitOfWork.Repository<Exam>()
                    .GetAll(cancellationToken)
                    .Where(e => e.Status != ExamStatus.Draft);

                if (request.UserId != Guid.Empty)
                {
                    exams = exams.WhereStudentEnrolled(request.UserId);
                }
            }

            var filters = new Dictionary<string, string>(request.RequestSkeleton.Filters, StringComparer.OrdinalIgnoreCase);

            if (!isInstructor)
            {
                if (filters.Remove("studentstatus", out var studentStatusStr) &&
                    Enum.TryParse<ExamResultStatus>(studentStatusStr, true, out var studentStatus))
                {
                    if (studentStatus == ExamResultStatus.NotStarted)
                    {
                        exams = exams.Where(e => !e.ExamResults.Any(r => r.StudentId == request.UserId && (r.Status != ExamResultStatus.NotStarted)));
                    }
                    else
                    {
                        exams = exams.Where(e => e.ExamResults.Any(r => r.StudentId == request.UserId && r.Status == studentStatus));
                    }
                }

                if (filters.Remove("istaken", out var isTakenStr) && bool.TryParse(isTakenStr, out var isTaken))
                {
                    if (isTaken)
                    {
                        exams = exams.Where(e => e.ExamResults.Any(r => r.StudentId == request.UserId && (r.Status != ExamResultStatus.NotStarted)));
                    }
                    else
                    {
                        exams = exams.Where(e => !e.ExamResults.Any(r => r.StudentId == request.UserId && (r.Status != ExamResultStatus.NotStarted)));
                    }
                }
            }
            else
            {
                filters.Remove("studentstatus");
                filters.Remove("istaken");
                filters.Remove("instructorid");
            }

            // Remove studentid from incoming filters as scope is already applied
            filters.Remove("studentid");

            exams = exams
                .ApplyFilters(filters, _examFilterRegistry.Filters)
                .ApplySort(request.RequestSkeleton.SortBy, request.RequestSkeleton.IsDescending, _examFilterRegistry.Sorts);

            var now = EgyptTime.UtcNow;
            IQueryable<ExamListDto> examsQuery = isInstructor
                ? exams.Select(e => new ExamListDto
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
                    StudentExamStatusResult = ExamResultStatus.NotStarted,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    IsTaken = false,
                    TotalMark = e.TotalMark,
                    NumberOfQuestions = e.NumberOfQuestions,
                    DurationInMinutes = e.DurationInMinutes,
                    IsRandomized = e.IsRandomized,
                    ExamType = e.ExamType,
                    PassMarkPercentage = e.PassMarkPercentage,
                    ObtainedMarks = 0m,
                    TakenAt = null,
                })
                : exams
                    .Select(e => new
                    {
                        Exam = e,
                        StudentResult = e.ExamResults.FirstOrDefault(se => se.StudentId == request.UserId)
                    })
                    .Select(x => new ExamListDto
                    {
                        ExamId = x.Exam.Id,
                        Name = x.Exam.Name,
                        Description = x.Exam.Description,
                        ExamStatus = x.Exam.Status == ExamStatus.Draft
                            ? ExamStatus.Draft
                            : (x.Exam.Status == ExamStatus.Finished || (x.Exam.EndTime != null && x.Exam.EndTime <= now)
                                ? ExamStatus.Finished
                                : (x.Exam.StartTime != null && x.Exam.StartTime <= now
                                    ? ExamStatus.Started
                                    : ExamStatus.Scheduled)),
                        StudentExamStatusResult = x.StudentResult != null ? x.StudentResult.Status : ExamResultStatus.NotStarted,
                        StartTime = x.Exam.StartTime,
                        EndTime = x.Exam.EndTime,
                        IsTaken = x.StudentResult != null &&
                            (x.StudentResult.Status == ExamResultStatus.InProgress || x.StudentResult.Status == ExamResultStatus.Passed || x.StudentResult.Status == ExamResultStatus.Failed),
                        TotalMark = x.Exam.TotalMark,
                        NumberOfQuestions = x.Exam.NumberOfQuestions,
                        DurationInMinutes = x.Exam.DurationInMinutes,
                        IsRandomized = x.Exam.IsRandomized,
                        ExamType = x.Exam.ExamType,
                        PassMarkPercentage = x.Exam.PassMarkPercentage,
                        ObtainedMarks = x.StudentResult != null ? (x.StudentResult.StudentMark ?? 0m) : 0m,
                        TakenAt = x.StudentResult != null ? x.StudentResult.TakenAt : null,
                    });

            var pageSize = request.RequestSkeleton.PageSize > 0
                ? request.RequestSkeleton.PageSize
                : 10;

            var paginatedItems = await examsQuery.ToPaginatedResultAsync(
                request.RequestSkeleton.PageNumber,
                pageSize,
                cancellationToken);

            return Result<PaginatedResult<ExamListDto>>.Success(paginatedItems);
        }
    }
}