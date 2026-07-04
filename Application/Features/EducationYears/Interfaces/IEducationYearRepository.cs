using Application.Common.Interfaces;
using Application.Features.EducationYears.DTOs;
using Domain.Entities;

namespace Application.Features.EducationYears.Interfaces
{
    public interface IEducationYearRepository : IRepository<EducationYear>
    {
        Task<List<EducationYearDto>> GetActiveEducationYearsForInstructorAsync(Guid? instructorId, string? applicationName = null);
        Task<EducationYearDto?> GetEducationYearByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAndIdAsync(string name, Guid excludeId, CancellationToken cancellationToken = default);
    }
}
