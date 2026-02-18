using Application.DTOs.Sections;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Sections.Commands.UpdateSection
{
    public record UpdateSectionCommand : IRequest<Result<SectionUpdateResponse>>
    {
        public Guid SectionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public Guid CourseId { get; set; }
    }


}
