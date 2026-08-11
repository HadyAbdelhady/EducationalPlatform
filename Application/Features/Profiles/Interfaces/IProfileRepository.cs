using Application.Features.Profiles.DTOs;

namespace Application.Features.Profiles.Interfaces
{
    public interface IProfileRepository
    {
        Task<bool> StudentExistsAsync(Guid studentId, CancellationToken cancellationToken = default);

        Task<bool> InstructorExistsAsync(Guid instructorId, CancellationToken cancellationToken = default);

        Task<bool> HasSharedContentAsync(
            Guid instructorId,
            Guid studentId,
            CancellationToken cancellationToken = default);

        Task<StudentProfileForInstructorResponse?> GetStudentProfileForInstructorAsync(
            Guid instructorId,
            Guid studentId,
            CancellationToken cancellationToken = default);

        Task<InstructorProfileForStudentResponse?> GetInstructorProfileForStudentAsync(
            Guid studentId,
            Guid instructorId,
            CancellationToken cancellationToken = default);
    }
}
