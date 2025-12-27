using Application.DTOs.Sections;
using Application.Features.Sections.Query.GetSectionDetails;
using Application.Features.Sections.Query.GetSectionsForCourse;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISectionRepository : IRepository<Section>
    {
        public Task<GetSectionDetailsResponse> GetSectionDetailsResponse(GetSectionDetailsQuery Request, CancellationToken cancellationToken);
        public Task<List<GetSectionResponse>> GetSectionInnerData(GetSectionsForCourseQuery Request, CancellationToken cancellationToken);
    }
}
