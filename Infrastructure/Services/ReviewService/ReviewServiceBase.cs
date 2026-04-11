using Application.DTOs.Review;
using Application.HelperFunctions;
using Application.Interfaces;
using Application.Interfaces.BaseFilters;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using Domain.Interfaces;

namespace Infrastructure.Services.ReviewService
{

    public class ReviewServiceBase<TReview>(IUnitOfWork unitOfWork, IBaseFilterRegistry<TReview> filterRegistry) : IReviewService where TReview : Review, new()
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IBaseFilterRegistry<TReview> _filterRegistry = filterRegistry;

        public virtual async Task<Result<ReviewResponse>> CreateReviewAsync(ReviewCreationRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var student = await _unitOfWork.GetRepository<IUserRepository>().DoesStudentExistAsync(request.StudentId, cancellationToken);
                if (!student)
                {
                    return Result<ReviewResponse>.FailureStatusCode("Student not found.", ErrorType.NotFound);
                }

                var reviewAlreadyExists = await _unitOfWork.Repository<TReview>().AnyAsync(r => r.StudentId == request.StudentId &&
                                                                                           r.EntityId == request.EntityId,
                                                                                           cancellationToken);
                if (reviewAlreadyExists)
                {
                    return Result<ReviewResponse>.FailureStatusCode("You have already submitted a review.", ErrorType.BadRequest);
                }

                TReview newReview = new()
                {
                    Id = Guid.NewGuid(),
                    Comment = request.Comment,
                    StarRating = request.StarRating,
                    StudentId = request.StudentId,
                    EntityId = request.EntityId,
                };

                await _unitOfWork.Repository<TReview>().AddAsync(newReview, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await UpdateEntityRatingAsync(request.EntityId, cancellationToken);

                return Result<ReviewResponse>.Success(new ReviewResponse
                {
                    ReviewId = newReview.Id,
                    Comment = request.Comment,
                    StarRating = request.StarRating

                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Result<ReviewResponse>.FailureStatusCode($"Unauthorized access: {ex.Message}", ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<ReviewResponse>.FailureStatusCode($"An error occurred while creating the review.{ex.Message}", ErrorType.InternalServerError);
            }
        }

        public virtual async Task<Result<string>> DeleteReviewAsync(Guid reviewId, CancellationToken cancellationToken = default)
        {
            try
            {
                var review = await _unitOfWork.Repository<TReview>().GetByIdAsync(reviewId, cancellationToken);
                var entityId = review?.EntityId;

                var reviewExists = await _unitOfWork.Repository<TReview>().AnyAsync(r => r.Id == reviewId, cancellationToken);
                if (!reviewExists)
                {
                    return Result<string>.FailureStatusCode($"Review not found", ErrorType.NotFound);
                }

                await _unitOfWork.Repository<TReview>().RemoveAsync(reviewId, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                if (entityId.HasValue)
                {
                    await UpdateEntityRatingAsync(entityId.Value, cancellationToken);
                }

                return Result<string>.Success($"Review with ID: {reviewId} deleted Successfully.");
            }
            catch (UnauthorizedAccessException authEx)
            {
                return Result<string>.FailureStatusCode(authEx.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<string>.FailureStatusCode($"Error deleting review: {ex.Message}", ErrorType.InternalServerError);
            }
        }

        public async Task<Result<ReviewResponse>> UpdateReviewAsync(ReviewUpdateRequest request, CancellationToken cancellationToken = default)
        {
            var review = await _unitOfWork.Repository<TReview>().GetByIdAsync(request.ReviewId, cancellationToken);
            if (review is null)
            {
                return Result<ReviewResponse>.FailureStatusCode($"Review not found", ErrorType.NotFound);
            }

            review.Comment = request.Comment;
            review.StarRating = request.StarRating;
            review.UpdatedAt = DateTimeOffset.UtcNow;

            _unitOfWork.Repository<TReview>().Update(review);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await UpdateEntityRatingAsync(review.EntityId, cancellationToken);

            return Result<ReviewResponse>.Success(new ReviewResponse
            {
                ReviewId = request.ReviewId,
                StarRating = request.StarRating,
                Comment = request.Comment,
            });
        }

        // Centralized rating update method for all review types
        protected virtual async Task UpdateEntityRatingAsync(Guid entityId, CancellationToken cancellationToken)
        {
            if (typeof(TReview) == typeof(CourseReview))
            {
                await UpdateRatingForEntity<Course, CourseReview>(
                    entityId,
                    (course, rating) =>
                    {
                        course.Rating = rating;
                        return course;
                    },
                    cancellationToken);
            }
            else if (typeof(TReview) == typeof(SectionReview))
            {
                await UpdateRatingForEntity<Section, SectionReview>(
                    entityId,
                    (section, rating) =>
                    {
                        section.Rating = rating;
                        return section;
                    },
                    cancellationToken);
            }
            else if (typeof(TReview) == typeof(InstructorReview))
            {
                var userRepository = _unitOfWork.GetRepository<IUserRepository>();

                var instructorUser =
                    await userRepository.GetInstructorByIdWithRelationsAsync(entityId, cancellationToken);

                var reviewRepository = _unitOfWork.Repository<InstructorReview>();
                var reviews = reviewRepository.Find(r => r.EntityId == entityId, cancellationToken);

                decimal? newRating = null;
                if (reviews.Any())
                {
                    newRating = reviews.Average(r => r.StarRating);
                }

                instructorUser!.Instructor!.Rating = newRating;
                userRepository.Update(instructorUser);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            else if (typeof(TReview) == typeof(VideoReview))
            {
                await UpdateRatingForEntity<Video, VideoReview>(
                    entityId,
                    (video, rating) =>
                    {
                        video.Rating = rating;
                        return video;
                    },
                    cancellationToken);
            }
        }

        // Helper method used only inside the base class for soft-deletable entities
        private async Task UpdateRatingForEntity<TEntity, TEntityReview>(
            Guid entityId,
            Func<TEntity, decimal?, TEntity> updateRatingAction,
            CancellationToken cancellationToken)
            where TEntityReview : Review
            where TEntity : class, ISoftDeletableEntity
        {
            var entityRepository = _unitOfWork.Repository<TEntity>();
            var reviewRepository = _unitOfWork.Repository<TEntityReview>();

            var entity = await entityRepository.GetByIdAsync(entityId, cancellationToken);
            if (entity == null) return;

            var reviews = reviewRepository.Find(r => r.EntityId == entityId, cancellationToken);

            decimal? newRating = null;
            if (reviews.Any())
            {
                newRating = reviews.Average(r => r.StarRating);
            }

            var updatedEntity = updateRatingAction(entity, newRating);
            entityRepository.Update(updatedEntity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task<Result<List<GetAllReviewsResponse>>> GetAllReviewsAsync(ReviewGettingRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var reviews = _unitOfWork.Repository<TReview>()
                    .Find(r => r.EntityId == request.EntityId,
                        cancellationToken, r => r.Student!.User!)
                    .ApplyFilters(request.GetAllEntityRequestSkeleton.Filters, _filterRegistry.Filters)
                    .ApplySort(request.GetAllEntityRequestSkeleton.SortBy, request.GetAllEntityRequestSkeleton.IsDescending, _filterRegistry.Sorts);

                var reviewsList = reviews.ToList();

                if (reviewsList.Count == 0)
                {
                    return Result<List<GetAllReviewsResponse>>.FailureStatusCode(
                        $"No reviews found for entity with ID {request.EntityId}.",
                        ErrorType.NotFound);
                }

                var response = reviewsList
                    .Select(r => new GetAllReviewsResponse
                    {
                        Id = r.Id,
                        StudentId = r.StudentId,
                        StarRating = r.StarRating,
                        Comment = r.Comment,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt ?? r.CreatedAt,
                        Student = r.Student?.User != null ? new StudentReviewInfo
                        {
                            StudentId = r.StudentId,
                            FullName = r.Student.User.FullName,
                            PersonalPictureUrl = r.Student.User.PersonalPictureUrl
                        } : null
                    }).ToList();

                return Result<List<GetAllReviewsResponse>>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<List<GetAllReviewsResponse>>.FailureStatusCode(
                    $"An error occurred while retrieving reviews: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }

        public virtual async Task<Result<GetReviewByIdResponse>> GetReviewByIdAsync(Guid reviewId, CancellationToken cancellationToken = default)
        {
            try
            {
                var review = await _unitOfWork.Repository<TReview>()
                    .GetByIdAsync(reviewId, cancellationToken, r => r.Student!.User!);

                if (review is null || review.IsDeleted)
                {
                    return Result<GetReviewByIdResponse>.FailureStatusCode(
                        $"Review with ID {reviewId} not found.",
                        ErrorType.NotFound);
                }

                var response = new GetReviewByIdResponse
                {
                    Id = review.Id,
                    StudentId = review.StudentId,
                    EntityId = review.EntityId,
                    StarRating = review.StarRating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt,
                    UpdatedAt = review.UpdatedAt ?? review.CreatedAt,
                    Student = review.Student?.User != null ? new StudentReviewInfo
                    {
                        StudentId = review.StudentId,
                        FullName = review.Student.User.FullName,
                        PersonalPictureUrl = review.Student.User.PersonalPictureUrl
                    } : null
                };

                return Result<GetReviewByIdResponse>.Success(response);
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
