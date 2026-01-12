using Application.DTOs.Review;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Reviews.Query.GetAllReviews
{
    public class GetAllReviewsQueryHandler(IReviewServiceFactory reviewServiceFactory)
        : IRequestHandler<GetAllReviewsQuery, Result<List<GetAllReviewsResponse>>>
    {
        private readonly IReviewServiceFactory _reviewServiceFactory = reviewServiceFactory;

        public async Task<Result<List<GetAllReviewsResponse>>> Handle(GetAllReviewsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IReviewService reviewService = _reviewServiceFactory.GetReviewService(request.EntityType);
                return await reviewService.GetAllReviewsAsync(new ReviewGettingRequest(
                    request.EntityId,
                    request.EntityType,
                    request.Filters,
                    request.SortBy,
                    request.IsDescending),
                    cancellationToken);
            }
            catch (NotImplementedException ex)
            {
                return Result<List<GetAllReviewsResponse>>.FailureStatusCode(
                    ex.Message,
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
