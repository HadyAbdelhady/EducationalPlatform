using Application.DTOs.Review;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Review.Commands.UpdateReview
{
    public class UpdateCourseReviewCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateCourseReviewCommand, Result<CourseReviewUpdateResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<CourseReviewUpdateResponse>> Handle(UpdateCourseReviewCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var courseReview = await _unitOfWork.Repository<CourseReview>().GetByIdAsync(request.CourseReviewId, cancellationToken);
                if(courseReview is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<CourseReviewUpdateResponse>.FailureStatusCode($"Course review not found", ErrorType.NotFound);
                }

                courseReview.Comment = request.Comment;
                courseReview.StarRating = request.StarRating;
                courseReview.UpdatedAt = DateTimeOffset.UtcNow;

                _unitOfWork.Repository<CourseReview>().Update(courseReview);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<CourseReviewUpdateResponse>.Success(new CourseReviewUpdateResponse
                {
                    CourseReviewId = request.CourseReviewId,
                    StarRating = request.StarRating,
                    Comment = request.Comment,
                });

            }
            catch (UnauthorizedAccessException authEx)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<CourseReviewUpdateResponse>.FailureStatusCode(authEx.Message, Domain.enums.ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<CourseReviewUpdateResponse>.FailureStatusCode($"Error updating course review: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
