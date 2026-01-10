using Application.DTOs.Sections;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Sections.Query.GetSectionDetails
{
    public class GetSectionDetailsQuery : IRequest<Result<SectionDetailsQueryModel>>
    {
        public Guid SectionId { get; set; }
        public Guid UserId { get; set; }

    }
}
