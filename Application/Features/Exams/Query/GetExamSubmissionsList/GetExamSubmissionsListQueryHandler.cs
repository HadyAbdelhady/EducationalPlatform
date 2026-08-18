using Application.Features.Exams.DTOs;
using Application.Common;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Exams.Query.GetExamSubmissionsList
{
    public class GetExamSubmissionsListQueryHandler(
        IUnitOfWork unitOfWork,
        IBaseFilterRegistry<StudentExamResult> studentExamResultFilterRegistry)
        : IRequestHandler<GetExamSubmissionsListQuery, Result<ExamSubmissionsListResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IBaseFilterRegistry<StudentExamResult> _studentExamResultFilterRegistry = studentExamResultFilterRegistry;

        public async Task<Result<ExamSubmissionsListResponse>> Handle(
            GetExamSubmissionsListQuery request,
            CancellationToken cancellationToken)
        {
            var exam = await _unitOfWork.Repository<Exam>()
                .GetByIdAsync(
                    request.ExamId,
                    cancellationToken,
                    e => e.Course!,
                    e => e.Course!.EducationYear,
                    e => e.Section!);

            if (exam == null)
            {
                return Result<ExamSubmissionsListResponse>.FailureStatusCode("Exam not found", ErrorType.NotFound);
            }

            var examContext = ExamSubmissionProjectionContext.FromExam(exam);

            var submissionsQuery = _unitOfWork.Repository<StudentExamResult>()
                .GetAll(cancellationToken)
                .Where(er => er.ExamId == request.ExamId)
                .ApplyFilters(request.RequestSkeleton.Filters, _studentExamResultFilterRegistry.Filters)
                .ApplySort(request.RequestSkeleton.SortBy, request.RequestSkeleton.IsDescending, _studentExamResultFilterRegistry.Sorts);

            var submissions = await submissionsQuery
                .Select(ExamSubmissionDtoMapping.Project(examContext))
                .ToPaginatedResultAsync(request.RequestSkeleton.PageNumber, 10, cancellationToken);

            return Result<ExamSubmissionsListResponse>.Success(new ExamSubmissionsListResponse
            {
                Exam = ExamSubmissionDtoMapping.ToExamDetails(exam),
                Submissions = submissions
            });
        }
    }
}
