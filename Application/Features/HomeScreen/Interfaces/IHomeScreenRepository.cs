using Application.Features.HomeScreen.DTOs;

namespace Application.Features.HomeScreen.Interfaces
{
    public interface IHomeScreenRepository
    {
        Task<StudentHomeScreenResponse?> GetStudentHomeScreenDataAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<InstructorDashboardResponse?> GetInstructorDashboardDataAsync(Guid instructorId, Guid? educationYearId = null, CancellationToken cancellationToken = default);
        Task<InstructorScheduleResponse> GetInstructorScheduleAsync(Guid instructorId, Guid educationYearId, int days, CancellationToken cancellationToken = default);
        Task<AttentionResponse> GetInstructorAttentionAsync(Guid instructorId, Guid educationYearId, CancellationToken cancellationToken = default);
        Task<AtRiskResponse> GetInstructorAtRiskAsync(Guid instructorId, Guid educationYearId, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<InstructorPaymentsResponse> GetInstructorPaymentsAsync(Guid instructorId, Guid educationYearId, int days, CancellationToken cancellationToken = default);
    }
}
