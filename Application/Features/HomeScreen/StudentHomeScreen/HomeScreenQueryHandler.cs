using Application.DTOs.HomeScreen;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.HomeScreen.StudentHomeScreen
{
    public class HomeScreenQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<HomeScreenQuery, Result<StudentHomeScreenResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<StudentHomeScreenResponse>> Handle(
            HomeScreenQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var homeScreenRepository = _unitOfWork.GetRepository<IHomeScreenRepository>();
                var result = await homeScreenRepository.GetStudentHomeScreenDataAsync(request.StudentId, cancellationToken)
                                                        ?? new StudentHomeScreenResponse();

                // Repository handles null case and returns response with videos
                return Result<StudentHomeScreenResponse>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<StudentHomeScreenResponse>.FailureStatusCode(
                    $"An error occurred while retrieving home screen data: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
