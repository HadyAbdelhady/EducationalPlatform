using Application.DTOs.HomeScreen;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.HomeScreen.InstructorDashboard
{
    public class InstructorDashboardQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<InstructorDashboardQuery, Result<InstructorDashboardResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<InstructorDashboardResponse>> Handle(
            InstructorDashboardQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var homeScreenRepository = _unitOfWork.GetRepository<IHomeScreenRepository>();
                var result = await homeScreenRepository.GetInstructorDashboardDataAsync(request.InstructorId, request.EducationYearId, cancellationToken)
                                                        ?? new InstructorDashboardResponse();

                return Result<InstructorDashboardResponse>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<InstructorDashboardResponse>.FailureStatusCode(
                    $"An error occurred while retrieving instructor dashboard data: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
