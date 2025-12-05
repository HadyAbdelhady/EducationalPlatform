using Application.DTOs.Videos;
using Application.ResultWrapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Videos.Commands.CreateVideo
{
    public record BulkCreateVideosCommand(List<VideoCreationRequest> Videos):IRequest<Result<List<VideoCreationResponse>>>
    {
    }
}
