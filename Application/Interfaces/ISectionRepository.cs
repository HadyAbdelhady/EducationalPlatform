using Application.DTOs.Sections;
using Application.Features.Sections.Query.GetSectionDetails;
using Application.Features.Sections.Query.GetSectionsForCourse;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISectionRepository : IRepository<Section>
    {
        public Task<SectionDetailsQueryModel> GetSectionDetailsResponse(GetSectionDetailsQuery Request, CancellationToken cancellationToken);
        public Task<List<SectionDetailsQueryModel>> GetSectionList(GetSectionsForCourseQuery Request, CancellationToken cancellationToken);
    }
}
