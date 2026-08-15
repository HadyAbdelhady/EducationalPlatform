using Application.Common;
using Application.Common.Interfaces;
using Application.Features.HomeScreen.Interfaces;
using Application.Features.Profiles.DTOs;
using Application.Features.Profiles.Interfaces;
using Application.Features.Reviews.DTOs;
using Application.Features.Reviews.Interfaces;
using Domain.enums;
using MediatR;

namespace Application.Features.Profiles.GetInstructorProfileForStudent
{
    public class GetInstructorProfileForStudentQueryHandler(
        IUnitOfWork unitOfWork,
        IReviewServiceFactory reviewServiceFactory)
        : IRequestHandler<GetInstructorProfileForStudentQuery, Result<InstructorProfileForStudentResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IReviewServiceFactory _reviewServiceFactory = reviewServiceFactory;

        public async Task<Result<InstructorProfileForStudentResponse>> Handle(
            GetInstructorProfileForStudentQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IProfileRepository>();

                if (!await repo.InstructorExistsAsync(request.InstructorId, cancellationToken))
                {
                    return Result<InstructorProfileForStudentResponse>.FailureStatusCode(
                        "Instructor not found.",
                        ErrorType.NotFound);
                }

                var profile = await repo.GetInstructorProfileForStudentAsync(
                    request.StudentId,
                    request.InstructorId,
                    cancellationToken);

                if (profile is null)
                {
                    return Result<InstructorProfileForStudentResponse>.FailureStatusCode(
                        "Instructor not found.",
                        ErrorType.NotFound);
                }

                var enrollmentRepo = _unitOfWork.GetRepository<IStudentEnrollmentRepository>();
                foreach (var item in profile.SharedContent.Where(c =>
                             c.CourseId.HasValue &&
                             !c.SectionId.HasValue &&
                             !c.IsEnrolled))
                {
                    item.Price = await enrollmentRepo.GetRemainingCoursePriceAsync(
                        request.StudentId,
                        item.CourseId!.Value,
                        item.Price ?? 0m,
                        cancellationToken);
                }

                var reviewService = _reviewServiceFactory.GetReviewService(ReviewEntityType.Instructor);
                var reviews = await reviewService.GetAllReviewsAsync(
                    new ReviewGettingRequest
                    {
                        EntityId = request.InstructorId,
                        EntityType = ReviewEntityType.Instructor,
                        GetAllEntityRequestSkeleton = new GetAllEntityRequestSkeleton
                        {
                            SortBy = "createdat",
                            IsDescending = true,
                            PageNumber = 1,
                            PageSize = 20
                        }
                    },
                    cancellationToken);

                if (reviews.IsFailure)
                {
                    return Result<InstructorProfileForStudentResponse>.FailureStatusCode(
                        reviews.Error ?? "An error occurred while retrieving reviews.",
                        reviews.ErrorType);
                }

                profile.Reviews = reviews.Value.Items.ToList();

                return Result<InstructorProfileForStudentResponse>.Success(profile);
            }
            catch (Exception ex)
            {
                return Result<InstructorProfileForStudentResponse>.FailureStatusCode(
                    $"An error occurred while retrieving the instructor profile: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
