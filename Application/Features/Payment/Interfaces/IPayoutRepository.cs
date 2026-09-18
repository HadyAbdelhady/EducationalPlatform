using Domain.Entities;

namespace Application.Features.Payment.Interfaces
{
    /// <summary>
    /// Queries specifically needed by the monthly payout job —
    /// extracted to avoid coupling the handler to EF and to bypass
    /// the IEntity constraint on the generic Repository.
    /// </summary>
    public interface IPayoutRepository
    {
        Task<List<Center>> GetAllActiveCentersAsync(CancellationToken ct = default);
        Task<List<Instructor>> GetAllIndependentInstructorsAsync(CancellationToken ct = default);
    }
}
