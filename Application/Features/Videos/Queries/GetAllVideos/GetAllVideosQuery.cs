using Application.DTOs;
using Application.DTOs.Videos;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Videos.Queries.GetAllVideos
{
    public class GetAllVideosQuery : IRequest<Result<PaginatedResult<VideoByUserIdResponse>>>
    {
        public GetAllEntityRequestSkeleton GetAllEntityRequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();
    }
}
