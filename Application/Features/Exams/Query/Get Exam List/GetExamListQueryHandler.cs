using Application.DTOs.Exam;
using Application.Features.Exams.Query.Get_Exam_List;
using Application.HelperFunctions;
using Application.Interfaces;
using Application.Interfaces.BaseFilters;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

public class GetAllExamsQueryHandler(IUnitOfWork unitOfWork,IBaseFilterRegistry<Exam> examFilterRegistry): IRequestHandler<GetAllExamsQuery, Result<PaginatedResult<ExamListDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IBaseFilterRegistry<Exam> _examFilterRegistry = examFilterRegistry;

    public async Task<Result<PaginatedResult<ExamListDto>>> Handle(
        GetAllExamsQuery request,
        CancellationToken cancellationToken)
    {
        var exams = _unitOfWork.Repository<Exam>()
            .GetAll(cancellationToken)
            .ApplyFilters(request.Filters, _examFilterRegistry.Filters)
            .ApplySort(request.SortBy, request.IsDescending, _examFilterRegistry.Sorts);

        var response = exams.Select(e => new ExamListDto
        {
            ExamId = e.Id,
            Name = e.Name,
            Description = e.Description,
            ExamStatus = e.Status,
            StudentExamStatusResult = e.ExamResults
                .Where(se => se.StudentId == request.UserId)
                .Select(se => (ExamResultStatus?)se.Status)
                .FirstOrDefault(),
            StartTime = e.StartTime,
            EndTime = e.EndTime,
            IsTaken = e.ExamResults.Any(),
            TotalMark = e.TotalMark,
            NumberOfQuestions = e.NumberOfQuestions,
            DurationInMinutes = e.DurationInMinutes,
            IsRandomized = e.IsRandomized,
            ExamType = e.ExamType,
            PassMarkPercentage = e.PassMarkPercentage,
            NotStartedCount = e.ExamResults
                .Count(r => r.Status == ExamResultStatus.NotStarted),

            InProgressCount = e.ExamResults
                .Count(r =>  r.Status == ExamResultStatus.InProgress),

            PassedCount = e.ExamResults
                .Count(r =>  r.Status == ExamResultStatus.Passed),

            FailedCount = e.ExamResults
                .Count(r => r.Status == ExamResultStatus.Failed),

            CompletedCount = e.ExamResults
                .Count(r => r.Status == ExamResultStatus.Passed || r.Status == ExamResultStatus.Failed)
        })
        .ToList();

        int pageSize = 10;
        int skip = (request.PageNumber - 1) * pageSize;

        return Result<PaginatedResult<ExamListDto>>.Success(
            new PaginatedResult<ExamListDto>
            {
                Items = response.Skip(skip).Take(pageSize).ToList(),
                PageNumber = request.PageNumber,
                PageSize = pageSize,
                TotalCount = response.Count
            });
    }
}
