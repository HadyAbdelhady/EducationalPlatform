using Application.DTOs.Course;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Course.Query.GetAllCourses
{
    public class GetAllCoursesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllCoursesQuery, List<CourseByUserIdResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<List<CourseByUserIdResponse>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
        {
            var courses = await _unitOfWork.Courses.GetAllAsync(cancellationToken);

            var response = courses.Select(course => new CourseByUserIdResponse
            {
                Id = course.Id,
                Title = course.Name,
                Price = (decimal)course.Price!,
                Rating = course.CourseReviews.Count > 0 ? course.CourseReviews.Average(r => r.StarRating) : 0,
                NumberOfStudents = course.StudentCourses?.Count ?? 0,
                NumberOfVideos = course.NumberOfVideos,
                NumberOfSections = course.Sections?.Count ?? 0,
                ThumbnailUrl = course.IntroVideoUrl!,
                CreatedAt = course.CreatedAt,
                UpdatedAt = course.UpdatedAt
            }).ToList();

            return response;
        }
    }
}
