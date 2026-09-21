using Domain.Entities;

namespace Application.Features.Students.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student?> GetStudentByIdAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task UpdateAsync(Student student, CancellationToken cancellationToken = default);
    }
}
