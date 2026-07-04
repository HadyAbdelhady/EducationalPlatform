using Application.Features.HomeScreen.DTOs;

namespace Application.Features.HomeScreen.Interfaces
{
    public interface IHomeScreenRepository
    {
        Task<StudentHomeScreenResponse?> GetStudentHomeScreenDataAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<InstructorDashboardResponse?> GetInstructorDashboardDataAsync(Guid instructorId, Guid? educationYearId = null, CancellationToken cancellationToken = default);
    }
}
