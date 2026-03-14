using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Reviews.Commands.DeleteReview
{
    public class DeleteReviewCommand : IRequest<Result<string>>
    {
        public Guid ReviewId { get; set; }
        public required ReviewEntityType EntityType { get; set; }
    }
}
