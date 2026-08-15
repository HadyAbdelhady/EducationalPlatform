using Application.Common.Interfaces;
using Application.Features.Reviews.Interfaces;
using Application.Common;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Reviews.Commands.DeleteReview
{
    public class DeleteReviewCommandHandler(IReviewServiceFactory reviewServiceFactory) : IRequestHandler<DeleteReviewCommand, Result<string>>
    {
        private readonly IReviewServiceFactory _reviewServiceFactory = reviewServiceFactory;

        public Task<Result<string>> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
        {
            var reviewService = _reviewServiceFactory.GetReviewService(request.EntityType);

            return reviewService.DeleteReviewAsync(request.ReviewId, request.StudentId, cancellationToken);
        }
    }
}
