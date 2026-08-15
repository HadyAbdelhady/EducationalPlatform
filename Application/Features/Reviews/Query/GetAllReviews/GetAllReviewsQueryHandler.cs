using Application.Features.Reviews.DTOs;
using Application.Features.Reviews.Query.GetAllReviews;
using Application.Features.Reviews.Interfaces;
using Application.Common;
using Domain.enums;
using MediatR;

namespace Application.Features.Review.Query.GetAllReviews
{
    public class GetAllReviewsQueryHandler(IReviewServiceFactory reviewServiceFactory)
        : IRequestHandler<GetAllReviewsQuery, Result<PaginatedResult<GetAllReviewsResponse>>>
    {
        private readonly IReviewServiceFactory _reviewServiceFactory = reviewServiceFactory;

        public async Task<Result<PaginatedResult<GetAllReviewsResponse>>> Handle(GetAllReviewsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IReviewService reviewService = _reviewServiceFactory.GetReviewService(request.EntityType);
                return await reviewService.GetAllReviewsAsync(new ReviewGettingRequest
                {
                    EntityId = request.EntityId,
                    EntityType = request.EntityType,
                    GetAllEntityRequestSkeleton = request.GetAllEntityRequestSkeleton,
                }, cancellationToken);

            }
            catch (NotImplementedException ex)
            {
                return Result<PaginatedResult<GetAllReviewsResponse>>.FailureStatusCode(
                    ex.Message,
                    ErrorType.BadRequest);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<GetAllReviewsResponse>>.FailureStatusCode(
                    $"An error occurred while retrieving reviews: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
