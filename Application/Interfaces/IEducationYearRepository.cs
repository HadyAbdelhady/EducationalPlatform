using Application.DTOs.EducationYear;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IEducationYearRepository : IRepository<EducationYear>
    {
        Task<List<EducationYearDto>> GetActiveEducationYearsAsync(CancellationToken cancellationToken = default);
        Task<EducationYearDto?> GetEducationYearByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAndIdAsync(string name, Guid excludeId, CancellationToken cancellationToken = default);
    }
}
