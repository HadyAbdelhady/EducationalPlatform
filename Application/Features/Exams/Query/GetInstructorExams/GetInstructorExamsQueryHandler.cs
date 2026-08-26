using Application.Features.Exams.DTOs;
using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Exams.Interfaces;
using Domain.enums;
using MediatR;

namespace Application.Features.Exams.Query.GetInstructorExams
{
    public class GetInstructorExamsQueryHandler(
        IUnitOfWork unitOfWork,
        IBaseFilterRegistry<InstructorExamsResponseDto> instructorExamsFilterRegistry) : IRequestHandler<GetInstructorExamsQuery, Result<InstructorExamsResult>>
    {
        private readonly IBaseFilterRegistry<InstructorExamsResponseDto> _instructorExamsFilterRegistry = instructorExamsFilterRegistry;
        private readonly IExamRepository _examRepository = unitOfWork.GetRepository<IExamRepository>();

        public async Task<Result<InstructorExamsResult>> Handle(
            GetInstructorExamsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var examsQuery = _examRepository.GetInstructorNonRandomExamsQuery(request.Request.InstructorId);

                var filteredSortedQuery = examsQuery
                    .ApplyFilters(request.Request.RequestSkeleton.Filters, _instructorExamsFilterRegistry.Filters)
                    .ApplySort(request.Request.RequestSkeleton.SortBy, request.Request.RequestSkeleton.IsDescending, _instructorExamsFilterRegistry.Sorts);

                var pageSize = request.Request.RequestSkeleton.PageSize > 0
                    ? request.Request.RequestSkeleton.PageSize
                    : 10;

                var paginatedExams = await filteredSortedQuery.ToPaginatedResultAsync(
                    request.Request.RequestSkeleton.PageNumber,
                    pageSize,
                    cancellationToken);

                var coursesSections = await _examRepository.GetInstructorCoursesSectionsHashMapAsync(
                    request.Request.InstructorId,
                    cancellationToken);

                return Result<InstructorExamsResult>.Success(
                    new InstructorExamsResult
                    {
                        Exams = paginatedExams,
                        CoursesSections = coursesSections
                    });
            }
            catch (Exception ex)
            {
                return Result<InstructorExamsResult>.FailureStatusCode(
                    $"An error occurred while retrieving instructor exams: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
