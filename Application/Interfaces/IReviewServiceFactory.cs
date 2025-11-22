using Domain.enums;

namespace Application.Interfaces
{
    public interface IReviewServiceFactory
    {
        public IReviewService GetReviewService(ReviewEntityType reviewEntityType);
    }
}
