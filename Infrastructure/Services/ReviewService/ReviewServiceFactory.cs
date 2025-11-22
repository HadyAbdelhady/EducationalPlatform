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
            switch (reviewEntityType)
            {
                case ReviewEntityType.Course:
                    return new CourseReviewService(unitOfWork);
                case ReviewEntityType.Section:
                    return new SectionReviewService(unitOfWork);
                case ReviewEntityType.Instructor:
                    return new InstructorReviewService(unitOfWork);
                case ReviewEntityType.Video:
                    return new VideoReviewService(unitOfWork);

                default:
                    throw new NotImplementedException();
            }
        }


    }

    public class VideoReviewService(IUnitOfWork unitOfWork) : IReviewService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;

        public Task<Result<ReviewResponse>> CreateReviewAsync(ReviewCreationRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteReviewAsync(Guid reviewId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<ReviewResponse>> UpdateReviewAsync(ReviewUpdateRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    public class InstructorReviewService(IUnitOfWork unitOfWork) : IReviewService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;

        public Task<Result<ReviewResponse>> CreateReviewAsync(ReviewCreationRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteReviewAsync(Guid reviewId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<ReviewResponse>> UpdateReviewAsync(ReviewUpdateRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    public class SectionReviewService(IUnitOfWork unitOfWork) : IReviewService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;

        public Task<Result<ReviewResponse>> CreateReviewAsync(ReviewCreationRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteReviewAsync(Guid reviewId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<ReviewResponse>> UpdateReviewAsync(ReviewUpdateRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    public class CourseReviewService(IUnitOfWork unitOfWork) : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<ReviewResponse>> CreateReviewAsync(ReviewCreationRequest request, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var course = await _unitOfWork.Repository<Course>().GetByIdAsync(request.EntityId, cancellationToken);
                var student = await _unitOfWork.Repository<User>().GetByIdAsync(request.StudentId, cancellationToken);
                if (course is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ReviewResponse>.FailureStatusCode("Course is not found.", ErrorType.NotFound);
                }
                if (student is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ReviewResponse>.FailureStatusCode("Student is not found.", ErrorType.NotFound);
                }

                var alreadyExistReview = await _unitOfWork.Repository<CourseReview>().AnyAsync(r => r.StudentId == request.StudentId && r.EntityId == request.EntityId);
                if (alreadyExistReview)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ReviewResponse>.FailureStatusCode("You have already submitted a review for this course.", ErrorType.NotFound);
                }

                CourseReview newCourseReview = new()
                {
                    Id = Guid.NewGuid(),
                    Comment = request.Comment,
                    StarRating = request.StarRating,
                    EntityId = request.EntityId,
                    StudentId = request.StudentId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                await _unitOfWork.Repository<CourseReview>().AddAsync(newCourseReview, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<ReviewResponse>.Success(new ReviewResponse
                {
                    CourseReviewId = newCourseReview.Id,
                    Comment = request.Comment,
                    StarRating = request.StarRating

                });
            }
            catch (UnauthorizedAccessException ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ReviewResponse>.FailureStatusCode($"Unauthorized access: {ex.Message}", ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ReviewResponse>.FailureStatusCode($"An error occurred while creating the review.{ex.Message}", ErrorType.BadRequest);
            }
        }

        public Task DeleteReviewAsync(Guid reviewId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<ReviewResponse>> UpdateReviewAsync(ReviewUpdateRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
