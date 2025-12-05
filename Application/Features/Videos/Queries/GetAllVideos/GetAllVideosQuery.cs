using Application.DTOs.Course;
using Application.DTOs.Videos;
using Application.ResultWrapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Videos.Queries.GetAllVideos
{
    public class GetAllVideosQuery : IRequest<Result<PaginatedResult<VideoByUserIdResponse>>>
    {
    }
}
