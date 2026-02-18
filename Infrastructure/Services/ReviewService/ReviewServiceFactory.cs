using Application.DTOs.Review;
using Application.Interfaces;
using Application.Interfaces.BaseFilters;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;

namespace Infrastructure.Services.ReviewService
{
    public class ReviewServiceFactory(
        IUnitOfWork unitOfWork,
        IBaseFilterRegistry<CourseReview> courseReviewFilterRegistry,
        IBaseFilterRegistry<SectionReview> sectionReviewFilterRegistry,
        IBaseFilterRegistry<InstructorReview> instructorReviewFilterRegistry,
        IBaseFilterRegistry<VideoReview> videoReviewFilterRegistry) : IReviewServiceFactory
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;
        private readonly IBaseFilterRegistry<CourseReview> _courseReviewFilterRegistry = courseReviewFilterRegistry;
        private readonly IBaseFilterRegistry<SectionReview> _sectionReviewFilterRegistry = sectionReviewFilterRegistry;
        private readonly IBaseFilterRegistry<InstructorReview> _instructorReviewFilterRegistry = instructorReviewFilterRegistry;
        private readonly IBaseFilterRegistry<VideoReview> _videoReviewFilterRegistry = videoReviewFilterRegistry;

        public IReviewService GetReviewService(ReviewEntityType reviewEntityType)
        {
            return reviewEntityType switch
            {
                ReviewEntityType.Course => new CourseReviewService(unitOfWork, _courseReviewFilterRegistry),
                ReviewEntityType.Section => new SectionReviewService(unitOfWork, _sectionReviewFilterRegistry),
                ReviewEntityType.Instructor => new InstructorReviewService(unitOfWork, _instructorReviewFilterRegistry),
                ReviewEntityType.Video => new VideoReviewService(unitOfWork, _videoReviewFilterRegistry),
                _ => throw new NotImplementedException(),
            };
        }

    }

    public class VideoReviewService(IUnitOfWork unitOfWork, IBaseFilterRegistry<VideoReview> filterRegistry)
        : ReviewServiceBase<VideoReview>(unitOfWork, filterRegistry)
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

    public class InstructorReviewService(IUnitOfWork unitOfWork, IBaseFilterRegistry<InstructorReview> filterRegistry)
        : ReviewServiceBase<InstructorReview>(unitOfWork, filterRegistry)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public override async Task<Result<ReviewResponse>> CreateReviewAsync(ReviewCreationRequest request, CancellationToken cancellationToken = default)
        {
            var instructor = await _unitOfWork.GetRepository<IUserRepository>().DoesInstructorExistAsync(request.EntityId, cancellationToken);
            if (!instructor)
                return Result<ReviewResponse>.FailureStatusCode("Instructor not found.", ErrorType.NotFound);

            return await base.CreateReviewAsync(request, cancellationToken);
        }
    }

    public class SectionReviewService(IUnitOfWork unitOfWork, IBaseFilterRegistry<SectionReview> filterRegistry)
        : ReviewServiceBase<SectionReview>(unitOfWork, filterRegistry)
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

    public class CourseReviewService(IUnitOfWork unitOfWork, IBaseFilterRegistry<CourseReview> filterRegistry)
        : ReviewServiceBase<CourseReview>(unitOfWork, filterRegistry)
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
