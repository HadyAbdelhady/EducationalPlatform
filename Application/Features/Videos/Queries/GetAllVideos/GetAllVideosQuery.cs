using Application.Common;
using Application.Features.Videos.DTOs;
using Application.Common;
using MediatR;

namespace Application.Features.Videos.Queries.GetAllVideos
{
    public class GetAllVideosQuery : IRequest<Result<PaginatedResult<VideoByUserIdResponse>>>
    {
        public GetAllEntityRequestSkeleton GetAllEntityRequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();
        public Guid? StudentId { get; set; }
    }
}
