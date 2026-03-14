using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Courses.Commands.DeleteCourse
{
    public class DeleteCourseCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCourseCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<string>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var courseRepo = _unitOfWork.Repository<Course>();

                var course = await courseRepo.GetByIdAsync(request.CourseId, cancellationToken)
                                                            ?? throw new KeyNotFoundException("Course not found");

                var hasEnrolledStudentsInCourse = await courseRepo
                    .AnyAsync(c => c.Id == request.CourseId && c.StudentCourses.Any(), cancellationToken);

                var hasEnrolledStudentsInAnySection = await courseRepo
                    .AnyAsync(c => c.Id == request.CourseId && c.Sections.Any(s => s.StudentSections.Any()), cancellationToken);

                if (hasEnrolledStudentsInCourse || hasEnrolledStudentsInAnySection)
                {
                    return Result<string>.FailureStatusCode(
                        "Cannot delete this course because there are students enrolled.",
                        ErrorType.Conflict);
                }

                await courseRepo.RemoveAsync(course.Id, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<string>.Success("Course deleted successfully");
            }
            catch (KeyNotFoundException knfEx)
            {
                return Result<string>.FailureStatusCode(knfEx.Message, ErrorType.NotFound);
            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<string>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<string>.FailureStatusCode($"An error occurred while deleting the course: {ex.Message}", ErrorType.InternalServerError);

            }
        }
    }
}
