using Application.DTOs.Videos;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Videos.Queries.GetVideoById
{
    public class GetVideoByIdQuery : IRequest<Result<VideoByUserIdResponse>>
    {
        public Guid VideoId { get; set; }
    }
}
