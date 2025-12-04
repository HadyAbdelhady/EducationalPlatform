using Application.DTOs.Review;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Review.Query.GetReviewById
{
    public class GetReviewByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetReviewByIdQuery, Result<GetReviewByIdResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<GetReviewByIdResponse>> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Try to find as CourseReview first
                var courseReview = await _unitOfWork.Repository<Domain.Entities.CourseReview>()
                    .GetByIdAsync(request.ReviewId, cancellationToken, r => r.Student!.User!);

                if (courseReview != null && !courseReview.IsDeleted)
                {
                    var response = new GetReviewByIdResponse
                    {
                        Id = courseReview.Id,
                        StudentId = courseReview.StudentId,
                        EntityId = courseReview.EntityId,
                        StarRating = courseReview.StarRating,
                        Comment = courseReview.Comment,
                        CreatedAt = courseReview.CreatedAt,
                        UpdatedAt = courseReview.UpdatedAt,
                        Student = courseReview.Student?.User != null ? new StudentReviewInfo
                        {
                            StudentId = courseReview.StudentId,
                            FullName = courseReview.Student.User.FullName,
                            PersonalPictureUrl = courseReview.Student.User.PersonalPictureUrl
                        } : null
                    };

                    return Result<GetReviewByIdResponse>.Success(response);
                }

                return Result<GetReviewByIdResponse>.FailureStatusCode(
                    $"Review with ID {request.ReviewId} not found.",
                    ErrorType.NotFound);
            }
            catch (Exception ex)
            {
                return Result<GetReviewByIdResponse>.FailureStatusCode(
                    $"An error occurred while retrieving the review: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}

