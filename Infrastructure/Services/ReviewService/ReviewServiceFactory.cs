using Application.DTOs.Review;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;

namespace Infrastructure.Services.ReviewService
{
    public class ReviewServiceFactory(IUnitOfWork unitOfWork) : IReviewServiceFactory
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;

        public IReviewService GetReviewService(ReviewEntityType reviewEntityType)
        {
            return reviewEntityType switch
            {
                ReviewEntityType.Course => new CourseReviewService(unitOfWork),
                ReviewEntityType.Section => new SectionReviewService(unitOfWork),
                ReviewEntityType.Instructor => new InstructorReviewService(unitOfWork),
                ReviewEntityType.Video => new VideoReviewService(unitOfWork),
                _ => throw new NotImplementedException(),
            };
        }

    }

    public class VideoReviewService(IUnitOfWork unitOfWork) : ReviewServiceBase<VideoReview>(unitOfWork)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public override async Task<Result<ReviewResponse>> CreateReviewAsync(ReviewCreationRequest request, CancellationToken cancellationToken = default)
        {
            var video = await _unitOfWork.Repository<Video>().AnyAsync(v => v.Id == request.EntityId, cancellationToken);
            if (!video)
                return Result<ReviewResponse>.FailureStatusCode("Video not found.", ErrorType.NotFound);

            return await base.CreateReviewAsync(request, cancellationToken);
        }
    }

    public class InstructorReviewService(IUnitOfWork unitOfWork) : ReviewServiceBase<InstructorReview>(unitOfWork)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public override async Task<Result<ReviewResponse>> CreateReviewAsync(ReviewCreationRequest request, CancellationToken cancellationToken = default)
        {
            var instructor = await _unitOfWork.Repository<User>().AnyAsync(i => i.Id == request.EntityId, cancellationToken);
            if (!instructor)
                return Result<ReviewResponse>.FailureStatusCode("Instructor not found.", ErrorType.NotFound);

            return await base.CreateReviewAsync(request, cancellationToken);
        }
    }

    public class SectionReviewService(IUnitOfWork unitOfWork) : ReviewServiceBase<SectionReview>(unitOfWork)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public override async Task<Result<ReviewResponse>> CreateReviewAsync(ReviewCreationRequest request, CancellationToken cancellationToken = default)
        {
            var section = await _unitOfWork.Repository<Section>().AnyAsync(s => s.Id == request.EntityId, cancellationToken);
            if (!section)
                return Result<ReviewResponse>.FailureStatusCode("Section not found.", ErrorType.NotFound);

            return await base.CreateReviewAsync(request, cancellationToken);
        }
    }

    public class CourseReviewService(IUnitOfWork unitOfWork) : ReviewServiceBase<CourseReview>(unitOfWork)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public override async Task<Result<ReviewResponse>> CreateReviewAsync(ReviewCreationRequest request, CancellationToken cancellationToken = default)
        {
            var courseExists = await _unitOfWork.Repository<Course>().AnyAsync(c => c.Id == request.EntityId, cancellationToken);
            if (!courseExists)
                return Result<ReviewResponse>.FailureStatusCode("Course not found.", ErrorType.NotFound);

            return await base.CreateReviewAsync(request, cancellationToken);
        }
    }
}
