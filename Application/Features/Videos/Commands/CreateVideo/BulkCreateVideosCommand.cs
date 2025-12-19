using Application.DTOs.Videos;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Videos.Commands.CreateVideo
{
    public record BulkCreateVideosCommand(List<VideoCreationRequest> Videos) : IRequest<Result<List<VideoCreationResponse>>>
    {
    }
}
