using Application.DTOs.Videos;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Videos.Commands.CreateVideo
{
    public record BulkCreateVideosCommand : IRequest<Result<List<VideoResponse>>>
    {
        public Guid SectionId { get; set; }
        public List<VideoBulkCreationRequest> Videos { get; set; } = [];
    }
}
