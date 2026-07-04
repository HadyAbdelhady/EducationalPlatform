using Application.Common.Interfaces;
using Application.Features.Sections.DTOs;
using Application.Features.Sections.Query.GetSectionDetails;
using Application.Features.Sections.Query.GetSectionsForCourse;
using Domain.Entities;

namespace Application.Features.Sections.Interfaces
{
    public interface ISectionRepository : IRepository<Section>
    {
        public Task<SectionDetailsQueryModel> GetSectionDetailsResponse(GetSectionDetailsQuery Request, CancellationToken cancellationToken);
        public Task<List<SectionDetailsQueryModel>> GetSectionList(GetSectionsForCourseQuery Request, CancellationToken cancellationToken);
    }
}
