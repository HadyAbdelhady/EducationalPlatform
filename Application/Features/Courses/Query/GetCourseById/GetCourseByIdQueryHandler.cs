using Application.ResultWrapper;
using Application.DTOs.Courses;
using Application.Interfaces;
using Domain.enums;
using MediatR;

namespace Application.Features.Courses.Query.GetCourseById
{
    public class GetCourseByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCourseByIdQuery, Result<CourseDetailResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<CourseDetailResponse>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _unitOfWork.GetRepository<ICourseRepository>()
                                                                 .GetCourseDetailResponseByIdAsync(request, cancellationToken)
                                                                        ?? throw new KeyNotFoundException($"Course with ID {request.CourseId} not found.");

                return Result<CourseDetailResponse>.Success(response);
            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<CourseDetailResponse>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<CourseDetailResponse>.FailureStatusCode($"An error occurred while retrieving the course: {ex.Message}", ErrorType.InternalServerError);
            }
        }

    }
}
