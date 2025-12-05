using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Course.Commands.DeleteCourse
{
    public class DeleteCourseCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCourseCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<string>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var course = await _unitOfWork.Repository<Domain.Entities.Course>().GetByIdAsync(request.CourseId, cancellationToken)
                                                            ?? throw new KeyNotFoundException("Course not found");

                await _unitOfWork.Repository<Domain.Entities.Course>().RemoveAsync(course.Id, cancellationToken);
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
