using Application.DTOs.HomeScreen;

namespace Application.Interfaces
{
    public interface IHomeScreenRepository
    {
        Task<StudentHomeScreenResponse?> GetStudentHomeScreenDataAsync(Guid studentId, CancellationToken cancellationToken = default);
    }
}

