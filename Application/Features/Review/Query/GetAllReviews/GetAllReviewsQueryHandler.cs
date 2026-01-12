using Application.DTOs.Review;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Review.Query.GetAllReviews
{
    public class GetAllReviewsQueryHandler(IReviewServiceFactory reviewServiceFactory) 
        : IRequestHandler<GetAllReviewsQuery, Result<List<GetAllReviewsResponse>>>
    {
        private readonly IReviewServiceFactory _reviewServiceFactory = reviewServiceFactory;

        public async Task<Result<List<GetAllReviewsResponse>>> Handle(GetAllReviewsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var reviewService = _reviewServiceFactory.GetReviewService(request.EntityType);
                return await reviewService.GetAllReviewsAsync(request.EntityId, cancellationToken);
            }
            catch (NotImplementedException)
            {
                return Result<List<GetAllReviewsResponse>>.FailureStatusCode(
                    $"Review entity type {request.EntityType} is not supported.",
                    ErrorType.BadRequest);
            }
            catch (Exception ex)
            {
                return Result<List<GetAllReviewsResponse>>.FailureStatusCode(
                    $"An error occurred while retrieving reviews: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
