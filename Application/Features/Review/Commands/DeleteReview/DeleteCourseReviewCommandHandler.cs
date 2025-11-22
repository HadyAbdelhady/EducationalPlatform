using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Review.Commands.DeleteReview
{
    public class DeleteCourseReviewCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCourseReviewCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<string>> Handle(DeleteCourseReviewCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var courseReview = await _unitOfWork.Repository<CourseReview>().GetByIdAsync(request.CourseReviewId, cancellationToken);
                if (courseReview is null)
                {
                    return Result<string>.FailureStatusCode($"Course review not found", ErrorType.NotFound);
                }

                await _unitOfWork.Repository<CourseReview>().RemoveAsync(courseReview.Id, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<string>.Success($"Course Review with ID: {request} deleted Successfully.");
            }
            catch (UnauthorizedAccessException authEx)
            {
                return Result<string>.FailureStatusCode(authEx.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<string>.FailureStatusCode($"Error deleting course review: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
