using Application.Features.Videos.DTOs;
using Application.Common;
using MediatR;

namespace Application.Features.Videos.Queries.GetVideoById
{
    public class GetVideoByIdQuery : IRequest<Result<VideoByUserIdResponse>>
    {
        public Guid VideoId { get; set; }
        public Guid? StudentId { get; set; }
    }
}
