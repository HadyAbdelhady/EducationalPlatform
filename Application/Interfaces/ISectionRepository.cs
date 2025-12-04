using Application.DTOs.Section;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISectionRepository : IRepository<Section>
    {
        public Task<GetSectionDetailsResponse> GetSectionDetailsResponse(Guid sectionId, CancellationToken cancellationToken);
    }
}
