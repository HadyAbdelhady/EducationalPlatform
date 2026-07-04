using Application.Features.Sections.DTOs;
using Application.Common;
using MediatR;

namespace Application.Features.Sections.Query.GetSectionDetails
{
    public class GetSectionDetailsQuery : IRequest<Result<SectionDetailsQueryModel>>
    {
        public Guid SectionId { get; set; }
        public Guid UserId { get; set; }

    }
}
