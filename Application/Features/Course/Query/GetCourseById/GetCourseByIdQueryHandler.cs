using Application.DTOs.Course;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Course.Query.GetCourseById
{
    public class GetCourseByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCourseByIdQuery, Result<CourseDetailResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<CourseDetailResponse>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch course with all related entities including nested navigation properties
                var course = await _unitOfWork.GetRepository<ICourseRepository>().GetCourseDetailByIdAsync(request.CourseId, cancellationToken)
                                  ?? throw new KeyNotFoundException($"Course with ID {request.CourseId} not found.");

                // Calculate rating
                var rating = course.CourseReviews.Count > 0
                    ? course.CourseReviews.Average(r => r.StarRating)
                    : 0;

                // Map instructors
                var instructors = course.InstructorCourses
                    .Where(ic => ic.Instructor?.User != null)
                    .Select(ic => new InstructorInfoDto
                    {
                        InstructorId = ic.InstructorId,
                        FullName = ic.Instructor.User.FullName,
                        PersonalPictureUrl = ic.Instructor.User.PersonalPictureUrl,
                        GmailExternal = ic.Instructor.User.GmailExternal
                    })
                    .ToList();

                // Map sections
                var sections = course.Sections
                    .Select(s => new SectionDetailDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Description = s.Description,
                        NumberOfVideos = s.NumberOfVideos,
                        Rating = s.Rating,
                        Price = s.Price
                    })
                    .ToList();

                // Map reviews
                var reviews = course.CourseReviews
                    .Where(r => r.Student?.User != null)
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new CourseReviewDto
                    {
                        Id = r.Id,
                        StarRating = r.StarRating,
                        Comment = r.Comment,
                        CreatedAt = r.CreatedAt,
                        Student = new StudentReviewInfoDto
                        {
                            StudentId = r.StudentId,
                            FullName = r.Student.User.FullName,
                            PersonalPictureUrl = r.Student.User.PersonalPictureUrl
                        }
                    })
                    .ToList();

                // Create response
                var response = new CourseDetailResponse
                {
                    Id = course.Id,
                    Title = course.Name,
                    Description = course.Description,
                    Price = course.Price ?? 0,
                    PictureUrl = course.PictureUrl,
                    IntroVideoUrl = course.IntroVideoUrl,
                    NumberOfVideos = course.NumberOfVideos,
                    Rating = rating,
                    TotalReviews = course.CourseReviews.Count,
                    NumberOfStudents = course.StudentCourses?.Count ?? 0,
                    NumberOfSections = sections.Count,
                    CreatedAt = course.CreatedAt,
                    UpdatedAt = course.UpdatedAt ?? course.CreatedAt,
                    Instructors = instructors,
                    Sections = sections,
                    Reviews = reviews
                };

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
