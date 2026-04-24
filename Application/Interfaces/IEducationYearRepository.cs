using Application.DTOs.EducationYear;

namespace Application.Interfaces
{
    public interface IEducationYearRepository
    {
        Task<List<EducationYearDto>> GetActiveEducationYearsAsync(CancellationToken cancellationToken = default);
        Task<EducationYearDto?> GetEducationYearByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAndIdAsync(string name, Guid excludeId, CancellationToken cancellationToken = default);
    }
}
