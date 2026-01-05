using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByGoogleEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User?> GetStudentByIdWithRelationsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<User?> GetInstructorByIdWithRelationsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<bool> DoesInstructorExistAsync(Guid instructorId, CancellationToken cancellationToken);
        Task<bool> DoesStudentExistAsync(Guid studentId, CancellationToken cancellationToken);
    }
}
