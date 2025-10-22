using Application.DTOs.Course;
using Application.Features.Course.Query.GetAllCoursesByInstructor;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Course.Commands.GetAllCoursesByInstructor
{
    public class GetAllCoursesByInstructorQueryHandler(
                                                        IUnitOfWork unitOfWork
                                                        ) : IRequestHandler<GetAllCoursesByInstructorQuery, List<CourseByInstructorResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<List<CourseByInstructorResponse>> Handle(GetAllCoursesByInstructorQuery request, CancellationToken cancellationToken)
        {
            var courses = await _unitOfWork.Courses.GetAllCoursesByInstructorIdAsync(request.InstructorId, cancellationToken);

            var response = courses.Select(course => new CourseByInstructorResponse
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
