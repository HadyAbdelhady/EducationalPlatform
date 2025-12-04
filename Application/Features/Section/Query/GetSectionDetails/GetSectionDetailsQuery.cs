using Application.DTOs.Section;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Section.Query.GetSectionDetails
{
    public class GetSectionDetailsQuery : IRequest<Result<GetSectionDetailsResponse>>
    {
        public Guid SectionId { get; set; }
    }
}
