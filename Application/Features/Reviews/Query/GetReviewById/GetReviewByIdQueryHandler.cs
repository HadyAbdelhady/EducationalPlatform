using Application.Features.Reviews.DTOs;
using Application.Common.Interfaces;
using Application.Features.Reviews.Interfaces;
using Application.Common;
using Domain.enums;
using MediatR;

namespace Application.Features.Review.Query.GetReviewById
{
    public class GetReviewByIdQueryHandler(IUnitOfWork unitOfWork, IReviewServiceFactory reviewServiceFactory) : IRequestHandler<GetReviewByIdQuery, Result<GetReviewByIdResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IReviewServiceFactory _reviewServiceFactory = reviewServiceFactory;


        public async Task<Result<GetReviewByIdResponse>> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IReviewService reviewService = _reviewServiceFactory.GetReviewService(request.EntityType);
                return await reviewService.GetReviewByIdAsync(request.ReviewId, cancellationToken);
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
