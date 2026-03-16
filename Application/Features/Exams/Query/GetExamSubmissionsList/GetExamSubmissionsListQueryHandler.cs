using Application.DTOs.Exam;
using Application.HelperFunctions;
using Application.Interfaces;
using Application.Interfaces.BaseFilters;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Exams.Query.GetExamSubmissionsList
{
    public class GetExamSubmissionsListQueryHandler(
        IUnitOfWork unitOfWork,
        IBaseFilterRegistry<StudentExamResult> studentExamResultFilterRegistry)
        : IRequestHandler<GetExamSubmissionsListQuery, Result<PaginatedResult<ExamSubmissionDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IBaseFilterRegistry<StudentExamResult> _studentExamResultFilterRegistry = studentExamResultFilterRegistry;

        public async Task<Result<PaginatedResult<ExamSubmissionDto>>> Handle(
            GetExamSubmissionsListQuery request,
            CancellationToken cancellationToken)
        {
            // Get the exam to verify it exists and get total marks
            var exam = await _unitOfWork.Repository<Exam>()
                .GetByIdAsync(request.ExamId, cancellationToken);

            if (exam == null)
            {
                return Result<PaginatedResult<ExamSubmissionDto>>.FailureStatusCode("Exam not found", ErrorType.NotFound);
            }

            // Get all student exam results for the specified exam
            var examResults = _unitOfWork.Repository<StudentExamResult>()
                .GetAll(cancellationToken)
                .Where(er => er.ExamId == request.ExamId)
                .ApplyFilters(request.RequestSkeleton.Filters, _studentExamResultFilterRegistry.Filters)
                .ApplySort(request.RequestSkeleton.SortBy, request.RequestSkeleton.IsDescending, _studentExamResultFilterRegistry.Sorts)
                .Select(er => new ExamSubmissionDto
                {
                    StudentId = er.StudentId,
                    StudentName = er.Student.User.FullName,
                    Status = er.Status,
                    ObtainedMarks = er.StudentMark,
                    TotalMark = exam.TotalMark,
                    TakenAt = er.TakenAt,
                    SubmittedAt = er.UpdatedAt ?? er.CreatedAt,
                    NumberOfAnswersSubmitted = er.StudentSubmissions.Count(),
                    TotalQuestions = exam.NumberOfQuestions,
                    IsCompleted = er.Status == ExamResultStatus.Passed || er.Status == ExamResultStatus.Failed
                });

            var resultList = examResults.ToList();

            // Apply pagination
            int pageSize = 10;
            int skip = (request.RequestSkeleton.PageNumber - 1) * pageSize;

            return Result<PaginatedResult<ExamSubmissionDto>>.Success(
                new PaginatedResult<ExamSubmissionDto>
                {
                    Items = [.. resultList.Skip(skip).Take(pageSize)],
                    PageNumber = request.RequestSkeleton.PageNumber,
                    PageSize = pageSize,
                    TotalCount = resultList.Count
                });
        }
    }
}
