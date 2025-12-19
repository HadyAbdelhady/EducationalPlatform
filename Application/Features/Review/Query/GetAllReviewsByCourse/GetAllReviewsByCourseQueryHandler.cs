using Application.DTOs.Review;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Review.Query.GetAllReviewsByCourse
{
    public class GetAllReviewsByCourseQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllReviewsByCourseQuery, Result<List<GetAllReviewsByCourseResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<List<GetAllReviewsByCourseResponse>>> Handle(GetAllReviewsByCourseQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var reviews = await _unitOfWork.Repository<CourseReview>()
                    .FindAsync(r => r.EntityId == request.CourseId && !r.IsDeleted, 
                        cancellationToken, r => r.Student!.User!);

                var reviewsList = reviews.ToList();

                if (reviewsList.Count == 0)
                {
                    return Result<List<GetAllReviewsByCourseResponse>>.FailureStatusCode(
                        $"No reviews found for course with ID {request.CourseId}.",
                        ErrorType.NotFound);
                }

                var response = reviewsList
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new GetAllReviewsByCourseResponse
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

                return Result<List<GetAllReviewsByCourseResponse>>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<List<GetAllReviewsByCourseResponse>>.FailureStatusCode(
                    $"An error occurred while retrieving reviews: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}

