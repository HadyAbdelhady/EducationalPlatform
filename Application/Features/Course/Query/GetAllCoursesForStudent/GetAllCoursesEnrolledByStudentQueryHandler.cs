using Application.DTOs.Course;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Course.Query.GetAllCoursesForStudent
{
    public class GetAllCoursesEnrolledByStudentQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllCoursesEnrolledByStudentQuery, List<CourseByUserIdResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<List<CourseByUserIdResponse>> Handle(GetAllCoursesEnrolledByStudentQuery request, CancellationToken cancellationToken)
        {
            var courses = await _unitOfWork.GetRepository<ICourseRepository>().GetAllCoursesByStudentIdAsync(request.StudentId, cancellationToken);

            var response = courses.Select(course => new CourseByUserIdResponse
            {
                Id = course.Id,
                Title = course.Name,
                Price = course.Price ?? 0,
                Rating = course.CourseReviews.Count > 0 ? course.CourseReviews.Average(r => r.StarRating) : 0,
                NumberOfStudents = course.StudentCourses?.Count ?? 0,
                NumberOfVideos = course.NumberOfVideos,
                NumberOfSections = course.Sections?.Count ?? 0,
                ThumbnailUrl = course.IntroVideoUrl ?? string.Empty,
                CreatedAt = course.CreatedAt,
                UpdatedAt = course.UpdatedAt
            }).ToList();

            return response;
        }
    }
}
