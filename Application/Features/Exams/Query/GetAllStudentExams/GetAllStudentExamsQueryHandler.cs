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
                .ApplyFilters(request.RequestSkeleton.Filters, _examFilterRegistry.Filters)
                .ApplySort(request.RequestSkeleton.SortBy, request.RequestSkeleton.IsDescending, _examFilterRegistry.Sorts);

            var examsQuery = exams.Select(e => new ExamListDto
            {
                ExamId = e.Id,
                Name = e.Name,
                Description = e.Description,
                ExamStatus = e.Status,
                StudentExamStatusResult = e.ExamResults
                    .Where(se => se.StudentId == request.UserId)
                    .Select(se => se.Status)
                    .FirstOrDefault(),
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                IsTaken = e.ExamResults.Any(se =>
                    se.StudentId == request.UserId),
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