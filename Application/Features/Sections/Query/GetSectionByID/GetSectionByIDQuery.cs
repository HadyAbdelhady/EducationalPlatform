using Application.DTOs.Sections;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Sections.Query.GetSectionByID
{
    public class GetSectionByIDQuery : IRequest<Result<GetSectionByIDResponse>>
    {
        public Guid SectionId { get; set; }
    }
}
