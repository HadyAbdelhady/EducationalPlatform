using Application.DTOs.Sections;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISectionRepository : IRepository<Section>
    {
        public Task<GetSectionDetailsResponse> GetSectionDetailsResponse(Guid sectionId, CancellationToken cancellationToken);
    }
}
