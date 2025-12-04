using Application.DTOs.Section;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Section.Query.GetSectionByID
{
    public class GetSectionByIDQuery : IRequest<Result<GetSectionByIDResponse>>
    {
        public Guid SectionId { get; set; }
    }
}
