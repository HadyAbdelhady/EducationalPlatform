using Application.DTOs.EducationYear;

namespace Application.Interfaces
{
    public interface IEducationYearRepository
    {
        Task<List<EducationYearDto>> GetActiveEducationYearsAsync(CancellationToken cancellationToken = default);
    }
}
