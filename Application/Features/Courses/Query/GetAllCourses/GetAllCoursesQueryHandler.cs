using Application.Common.Interfaces;
using Application.Features.EducationYears.Interfaces;
using Application.Common;
using Application.Features.Courses.DTOs;
using Application.Features.Courses.Interfaces;
using Application.Features.HomeScreen.Interfaces;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Courses.Query.GetAllCourses
{
    public class GetAllCoursesQueryHandler(IUnitOfWork unitOfWork,
                                            IBaseFilterRegistry<Course> courseFilterRegistry,
                                            IStudentEducationYearProvider studentEducationYearProvider) : IRequestHandler<GetAllCoursesQuery,
                                                                                                                           Result<PaginatedResult<CourseResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IBaseFilterRegistry<Course> _courseFilterRegistry = courseFilterRegistry;
        private readonly IStudentEducationYearProvider _studentEducationYearProvider = studentEducationYearProvider;

        public async Task<Result<PaginatedResult<CourseResponse>>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var paginatedResponse = await _unitOfWork.GetRepository<ICourseRepository>()
                    .GetCoursesPaginatedAsync(request, _courseFilterRegistry, _studentEducationYearProvider, cancellationToken);

                var enrollmentRepo = _unitOfWork.GetRepository<IStudentEnrollmentRepository>();
                foreach (var course in paginatedResponse.Items.Where(c => !c.IsEnrolled))
                {
                    course.Price = await enrollmentRepo.GetRemainingCoursePriceAsync(
                        request.UserID,
                        course.Id,
                        course.Price,
                        cancellationToken);
                }

                return Result<PaginatedResult<CourseResponse>>.Success(paginatedResponse);
            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<PaginatedResult<CourseResponse>>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<CourseResponse>>.FailureStatusCode($"An error occurred while retrieving courses: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
