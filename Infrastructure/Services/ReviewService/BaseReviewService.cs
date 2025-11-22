using Application.DTOs.Review;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using FluentValidation;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.ReviewService
{
    public abstract class BaseReviewService<TReview> : IReviewService
                                                where TReview : Review, new()
    {
        protected readonly IUnitOfWork _unitOfWork;
        public abstract string SupportedEntityType { get; }

        protected BaseReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ReviewResponse>> CreateReviewAsync(ReviewCreationRequest request, CancellationToken cancellationToken = default)
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

            var alreadyExistReview = await _unitOfWork.Repository<CourseReview>().AnyAsync(r => r.StudentId == request.StudentId && r.CourseId == request.CourseId);
            if (alreadyExistReview)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ReviewResponse>.FailureStatusCode("You have already submitted a review for this course.", ErrorType.NotFound);
            }

            var review = new TReview
            {
                Id = new Guid(),
                StudentId = request.StudentId,
                EntityId = request.EntityId,
                EntityType = SupportedEntityType,
                StarRating = request.StarRating,
                Comment = request.Comment,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _unitOfWork.Repository<TReview>().AddAsync(review, cancellationToken);
        }

        public async Task DeleteReviewAsync(Guid reviewId, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.Repository<TReview>().RemoveAsync(reviewId, cancellationToken);
        }

        public async Task<Result<ReviewResponse>> UpdateReviewAsync(ReviewUpdateRequest request, CancellationToken cancellationToken = default)
        {
            var courseReview = await _unitOfWork.Repository<TReview>().GetByIdAsync(request.CourseReviewId, cancellationToken);
            if (courseReview is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ReviewResponse>.FailureStatusCode($"Course review not found", ErrorType.NotFound);
            }

            courseReview.Comment = request.Comment;
            courseReview.StarRating = request.StarRating;
            courseReview.UpdatedAt = DateTimeOffset.UtcNow;

            _unitOfWork.Repository<TReview>().Update(courseReview);
        }
    }
}
