using Application.DTOs.Review;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using Infrastructure.Repositories;
using MediatR;
using System.Threading;

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

    public abstract class ReviewServiceBase<TReview> : IReviewService where TReview : Review, new()
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewServiceBase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Result<ReviewResponse>> CreateReviewAsync(ReviewCreationRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<string>> DeleteReviewAsync(Guid reviewId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<ReviewResponse>> UpdateReviewAsync(ReviewUpdateRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    public class VideoReviewService(IUnitOfWork unitOfWork) : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<ReviewResponse>> CreateReviewAsync(ReviewCreationRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var video = await _unitOfWork.Repository<Video>().AnyAsync(v => v.Id == request.EntityId, cancellationToken);
                var student = await _unitOfWork.Repository<User>().AnyAsync(s => s.Id == request.StudentId, cancellationToken);
                if (!video)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ReviewResponse>.FailureStatusCode("Video not found.", ErrorType.NotFound);
                }
                if (!student)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ReviewResponse>.FailureStatusCode("Student not found.", ErrorType.NotFound);
                }

                var alreadyExistReview = await _unitOfWork.Repository<VideoReview>().AnyAsync(r => r.StudentId == request.StudentId && r.EntityId == request.EntityId);
                if (alreadyExistReview)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ReviewResponse>.FailureStatusCode("You have already submitted a review for this video.", ErrorType.BadRequest);
                }

                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                VideoReview newVideoreview = new()
                {
                    Id = Guid.NewGuid(),
                    Comment = request.Comment,
                    StarRating = request.StarRating,
                    EntityId = request.EntityId,
                    StudentId = request.StudentId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow,
                };

                await _unitOfWork.Repository<VideoReview>().AddAsync(newVideoreview, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<ReviewResponse>.Success(new ReviewResponse
                {
                    ReviewId = newVideoreview.Id,
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
                return Result<ReviewResponse>.FailureStatusCode($"An error occurred while creating the review.{ex.Message}", ErrorType.InternalServerError);
            }
        }

        public async Task<Result<string>> DeleteReviewAsync(Guid reviewId, CancellationToken cancellationToken = default)
        {
            try
            {
                var isVideoReviewExists = await _unitOfWork.Repository<VideoReview>().AnyAsync(r => r.Id == reviewId, cancellationToken);
                if (!isVideoReviewExists)
                {
                    return Result<string>.FailureStatusCode($"Video review not found", ErrorType.NotFound);
                }

                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                await _unitOfWork.Repository<VideoReview>().RemoveAsync(reviewId, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<string>.Success($"Video Review with ID: {reviewId} deleted Successfully.");
            }
            catch (UnauthorizedAccessException authEx)
            {
                return Result<string>.FailureStatusCode(authEx.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<string>.FailureStatusCode($"Error deleting video review: {ex.Message}", ErrorType.InternalServerError);
            }
        }

        public async Task<Result<ReviewResponse>> UpdateReviewAsync(ReviewUpdateRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var videoReview = await _unitOfWork.Repository<VideoReview>().GetByIdAsync(request.ReviewId, cancellationToken);
                if (videoReview is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ReviewResponse>.FailureStatusCode($"Video review not found", ErrorType.NotFound);
                }

                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                videoReview.Comment = request.Comment;
                videoReview.StarRating = request.StarRating;
                videoReview.UpdatedAt = DateTimeOffset.UtcNow;

                _unitOfWork.Repository<VideoReview>().Update(videoReview);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<ReviewResponse>.Success(new ReviewResponse
                {
                    ReviewId = request.ReviewId,
                    StarRating = request.StarRating,
                    Comment = request.Comment,
                });

            }
            catch (UnauthorizedAccessException authEx)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ReviewResponse>.FailureStatusCode(authEx.Message, Domain.enums.ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ReviewResponse>.FailureStatusCode($"Error updating video review: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }

    public class InstructorReviewService(IUnitOfWork unitOfWork) : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<ReviewResponse>> CreateReviewAsync(ReviewCreationRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var instructor = await _unitOfWork.Repository<User>().AnyAsync(i => i.Id == request.EntityId, cancellationToken);
                var student = await _unitOfWork.Repository<User>().AnyAsync(s => s.Id == request.StudentId, cancellationToken);
                if (!instructor)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ReviewResponse>.FailureStatusCode("Instructor not found.", ErrorType.NotFound);
                }
                if (!student)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ReviewResponse>.FailureStatusCode("Student not found.", ErrorType.NotFound);
                }

                var alreadyExistReview = await _unitOfWork.Repository<InstructorReview>().AnyAsync(r => r.StudentId == request.StudentId && r.EntityId == request.EntityId);
                if (alreadyExistReview)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ReviewResponse>.FailureStatusCode("You have already submitted a review for this instructor.", ErrorType.BadRequest);
                }

                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                InstructorReview newInstructoreview = new()
                {
                    Id = Guid.NewGuid(),
                    Comment = request.Comment,
                    StarRating = request.StarRating,
                    EntityId = request.EntityId,
                    StudentId = request.StudentId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow,
                };

                await _unitOfWork.Repository<InstructorReview>().AddAsync(newInstructoreview, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<ReviewResponse>.Success(new ReviewResponse
                {
                    ReviewId = newInstructoreview.Id,
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
                return Result<ReviewResponse>.FailureStatusCode($"An error occurred while creating the review.{ex.Message}", ErrorType.InternalServerError);
            }
        }

        public async Task<Result<string>> DeleteReviewAsync(Guid reviewId, CancellationToken cancellationToken = default)
        {
            try
            {
                var isInstructorReviewExists = await _unitOfWork.Repository<InstructorReview>().AnyAsync(r => r.Id == reviewId, cancellationToken);
                if (!isInstructorReviewExists)
                {
                    return Result<string>.FailureStatusCode($"Instructor review not found", ErrorType.NotFound);
                }

                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                await _unitOfWork.Repository<InstructorReview>().RemoveAsync(reviewId, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<string>.Success($"Instructor Review with ID: {reviewId} deleted Successfully.");
            }
            catch (UnauthorizedAccessException authEx)
            {
                return Result<string>.FailureStatusCode(authEx.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<string>.FailureStatusCode($"Error deleting instructor review: {ex.Message}", ErrorType.InternalServerError);
            }
        }

        public async Task<Result<ReviewResponse>> UpdateReviewAsync(ReviewUpdateRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var instructorReview = await _unitOfWork.Repository<InstructorReview>().GetByIdAsync(request.ReviewId, cancellationToken);
                if (instructorReview is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ReviewResponse>.FailureStatusCode($"Instructor review not found", ErrorType.NotFound);
                }

                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                instructorReview.Comment = request.Comment;
                instructorReview.StarRating = request.StarRating;
                instructorReview.UpdatedAt = DateTimeOffset.UtcNow;

                _unitOfWork.Repository<InstructorReview>().Update(instructorReview);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<ReviewResponse>.Success(new ReviewResponse
                {
                    ReviewId = request.ReviewId,
                    StarRating = request.StarRating,
                    Comment = request.Comment,
                });

            }
            catch (UnauthorizedAccessException authEx)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ReviewResponse>.FailureStatusCode(authEx.Message, Domain.enums.ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ReviewResponse>.FailureStatusCode($"Error updating instructor review: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }

    public class SectionReviewService(IUnitOfWork unitOfWork) : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<ReviewResponse>> CreateReviewAsync(ReviewCreationRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var section = await _unitOfWork.Repository<Section>().AnyAsync(s => s.Id == request.EntityId, cancellationToken);
                var student = await _unitOfWork.Repository<User>().AnyAsync(s => s.Id == request.StudentId, cancellationToken);
                if (!section)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ReviewResponse>.FailureStatusCode("Section not found.", ErrorType.NotFound);
                }
                if (!student)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ReviewResponse>.FailureStatusCode("Student not found.", ErrorType.NotFound);
                }

                var alreadyExistReview = await _unitOfWork.Repository<SectionReview>().AnyAsync(r => r.StudentId == request.StudentId && r.EntityId == request.EntityId);
                if (alreadyExistReview)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ReviewResponse>.FailureStatusCode("You have already submitted a review for this section.", ErrorType.BadRequest);
                }

                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                SectionReview newSectionReview = new()
                {
                    Id = Guid.NewGuid(),
                    Comment = request.Comment,
                    StarRating = request.StarRating,
                    EntityId = request.EntityId,
                    StudentId = request.StudentId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow,
                };

                await _unitOfWork.Repository<SectionReview>().AddAsync(newSectionReview, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<ReviewResponse>.Success(new ReviewResponse
                {
                    ReviewId = newSectionReview.Id,
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
                return Result<ReviewResponse>.FailureStatusCode($"An error occurred while creating the review.{ex.Message}", ErrorType.InternalServerError);
            }
        }

        public async Task<Result<string>> DeleteReviewAsync(Guid reviewId, CancellationToken cancellationToken = default)
        {
            try
            {
                var isSectionReviewExists = await _unitOfWork.Repository<SectionReview>().AnyAsync(r => r.Id == reviewId, cancellationToken);
                if (!isSectionReviewExists)
                {
                    return Result<string>.FailureStatusCode($"Section review not found", ErrorType.NotFound);
                }

                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                await _unitOfWork.Repository<SectionReview>().RemoveAsync(reviewId, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<string>.Success($"Section Review with ID: {reviewId} deleted Successfully.");
            }
            catch (UnauthorizedAccessException authEx)
            {
                return Result<string>.FailureStatusCode(authEx.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<string>.FailureStatusCode($"Error deleting section review: {ex.Message}", ErrorType.InternalServerError);
            }
        }

        public async Task<Result<ReviewResponse>> UpdateReviewAsync(ReviewUpdateRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var sectionReview = await _unitOfWork.Repository<SectionReview>().GetByIdAsync(request.ReviewId, cancellationToken);
                if (sectionReview is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ReviewResponse>.FailureStatusCode($"Section review not found", ErrorType.NotFound);
                }

                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                sectionReview.Comment = request.Comment;
                sectionReview.StarRating = request.StarRating;
                sectionReview.UpdatedAt = DateTimeOffset.UtcNow;

                _unitOfWork.Repository<SectionReview>().Update(sectionReview);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<ReviewResponse>.Success(new ReviewResponse
                {
                    ReviewId = request.ReviewId,
                    StarRating = request.StarRating,
                    Comment = request.Comment,
                });

            }
            catch (UnauthorizedAccessException authEx)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ReviewResponse>.FailureStatusCode(authEx.Message, Domain.enums.ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ReviewResponse>.FailureStatusCode($"Error updating section review: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }

    public class CourseReviewService(IUnitOfWork unitOfWork) : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<ReviewResponse>> CreateReviewAsync(ReviewCreationRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var courseExists = await _unitOfWork.Repository<Course>().AnyAsync(c => c.Id == request.EntityId, cancellationToken);
                var studentExists = await _unitOfWork.Repository<User>().AnyAsync(s => s.Id == request.StudentId, cancellationToken);
                if (!courseExists)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ReviewResponse>.FailureStatusCode("Course not found.", ErrorType.NotFound);
                }
                if (!studentExists)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ReviewResponse>.FailureStatusCode("Student not found.", ErrorType.NotFound);
                }

                var alreadyExistReview = await _unitOfWork.Repository<CourseReview>().AnyAsync(r => r.StudentId == request.StudentId && r.EntityId == request.EntityId);
                if (alreadyExistReview)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ReviewResponse>.FailureStatusCode("You have already submitted a review for this course.", ErrorType.BadRequest);
                }

                // begin of transaction
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                CourseReview newCourseReview = new()
                {
                    Id = Guid.NewGuid(),
                    Comment = request.Comment,
                    StarRating = request.StarRating,
                    EntityId = request.EntityId,
                    StudentId = request.StudentId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow,
                };

                await _unitOfWork.Repository<CourseReview>().AddAsync(newCourseReview, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<ReviewResponse>.Success(new ReviewResponse
                {
                    ReviewId = newCourseReview.Id,
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
                return Result<ReviewResponse>.FailureStatusCode($"An error occurred while creating the review.{ex.Message}", ErrorType.InternalServerError);
            }
        }

        public async Task<Result<string>> DeleteReviewAsync(Guid reviewId, CancellationToken cancellationToken = default)
        {
            try
            {
                var isCourseReviewExists = await _unitOfWork.Repository<CourseReview>().AnyAsync(r => r.Id == reviewId, cancellationToken);
                if (!isCourseReviewExists)
                {
                    return Result<string>.FailureStatusCode($"Course review not found", ErrorType.NotFound);
                }

                await _unitOfWork.BeginTransactionAsync(cancellationToken);
                await _unitOfWork.Repository<CourseReview>().RemoveAsync(reviewId, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<string>.Success($"Course Review with ID: {reviewId} deleted Successfully.");
            }
            catch (UnauthorizedAccessException authEx)
            {
                return Result<string>.FailureStatusCode(authEx.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<string>.FailureStatusCode($"Error deleting course review: {ex.Message}", ErrorType.InternalServerError);
            }
        }

        public async Task<Result<ReviewResponse>> UpdateReviewAsync(ReviewUpdateRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var courseReview = await _unitOfWork.Repository<CourseReview>().GetByIdAsync(request.ReviewId, cancellationToken);
                if (courseReview is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ReviewResponse>.FailureStatusCode($"Course review not found", ErrorType.NotFound);
                }

                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                courseReview.Comment = request.Comment;
                courseReview.StarRating = request.StarRating;
                courseReview.UpdatedAt = DateTimeOffset.UtcNow;

                _unitOfWork.Repository<CourseReview>().Update(courseReview);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<ReviewResponse>.Success(new ReviewResponse
                {
                    ReviewId = request.ReviewId,
                    StarRating = request.StarRating,
                    Comment = request.Comment,
                });

            }
            catch (UnauthorizedAccessException authEx)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ReviewResponse>.FailureStatusCode(authEx.Message, Domain.enums.ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ReviewResponse>.FailureStatusCode($"Error updating course review: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
