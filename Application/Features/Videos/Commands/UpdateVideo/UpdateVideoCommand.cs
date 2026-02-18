using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Videos.Commands.UpdateVideo
{
    public record UpdateVideoCommand : IRequest<Result<string>>
    {
        public Guid VideoId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;

        public string? Description { get; set; }
        public Guid? SectionId { get; set; }
    }
}
