using Application.DTOs.Sections;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Sections.Commands.CreateSection
{
    public record BulkCreateSectionCommand: IRequest<Result<List<CreateSectionResponse>>>
    {
        public Guid CourseId { get; set; }
        public List<BulkSectionDataForCreation> Sections { get; set; } = [];

    }
}
