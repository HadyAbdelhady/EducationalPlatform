using Domain.enums;

namespace Application.Features.Reviews.Interfaces
{
    public interface IReviewServiceFactory
    {
        public IReviewService GetReviewService(ReviewEntityType reviewEntityType);
    }
}
