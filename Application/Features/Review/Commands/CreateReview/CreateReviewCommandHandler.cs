using Application.DTOs.Review;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Review.Commands.CreateReview
{
    public class CreateReviewCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateReviewCommand, Result<ReviewResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<ReviewResponse>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                
                return Result<ReviewResponse>.Success(new ReviewResponse
                {
                    
                });
            }
            catch(UnauthorizedAccessException auth)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ReviewResponse>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch(Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ReviewResponse>.FailureStatusCode($"Error creating course review: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}

//try
//{
//    var course = await _unitOfWork.Repository<Domain.Entities.Course>().GetByIdAsync(request.CourseId, cancellationToken);
//    var student = await _unitOfWork.Repository<User>().GetByIdAsync(request.StudentId, cancellationToken);
//    if (course is null)
//    {
//        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
//        return Result<CourseReviewCreationResponse>.FailureStatusCode("Course is not found.", ErrorType.NotFound);
//    }
//    if (student is null)
//    {
//        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
//        return Result<CourseReviewCreationResponse>.FailureStatusCode("Student is not found.", ErrorType.NotFound);
//    }

//    var alreadyExistReview = await _unitOfWork.Repository<CourseReview>().AnyAsync(r => r.StudentId == request.StudentId && r.CourseId == request.CourseId);
//    if(alreadyExistReview)
//    {
//        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
//        return Result<CourseReviewCreationResponse>.FailureStatusCode("You have already submitted a review for this course.", ErrorType.NotFound);
//    }

//    CourseReview newCourseReview = new()
//    {
//        Id = Guid.NewGuid(),
//        Comment = request.Comment,
//        StarRating = request.StarRating,
//        CourseId = request.CourseId,
//        StudentId = request.StudentId,
//        CreatedAt = DateTime.UtcNow,
//        UpdatedAt = DateTime.UtcNow,
//    };

//    await _unitOfWork.Repository<CourseReview>().AddAsync(newCourseReview, cancellationToken);

//    await _unitOfWork.SaveChangesAsync(cancellationToken);
//    await _unitOfWork.CommitTransactionAsync(cancellationToken);

//    return Result<CourseReviewCreationResponse>.Success(new CourseReviewCreationResponse
//    {
//        CourseReviewId = newCourseReview.Id,
//        Comment = request.Comment,
//        StarRating = request.StarRating
//    });
//}
