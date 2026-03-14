using Application.DTOs.Review;
using Application.Interfaces;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Reviews.Commands.CreateReview
{
    public class CreateReviewCommandHandler(IReviewServiceFactory reviewServiceFactory) : IRequestHandler<CreateReviewCommand, Result<ReviewResponse>>
    {
        protected readonly IReviewServiceFactory _reviewServiceFactory = reviewServiceFactory;

        public async Task<Result<ReviewResponse>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            IReviewService reviewService = _reviewServiceFactory.GetReviewService(request.EntityType);

            return await reviewService.CreateReviewAsync(new ReviewCreationRequest
            {
                StudentId = request.StudentId,
                EntityId = request.EntityId,
                EntityType = request.EntityType,
                StarRating = request.StarRating,
                Comment = request.Comment
            }, cancellationToken);
        }
    }
}