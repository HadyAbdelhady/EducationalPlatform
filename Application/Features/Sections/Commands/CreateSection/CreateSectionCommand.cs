using Application.DTOs.Sections;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Sections.Commands.CreateSection
{
    public record CreateSectionCommand : IRequest<Result<CreateSectionResponse>>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int NumberOfVideos { get; set; }
        public Guid CourseId { get; set; }
    }
}
