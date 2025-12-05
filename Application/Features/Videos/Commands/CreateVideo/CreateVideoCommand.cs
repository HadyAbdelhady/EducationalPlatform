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
    public record CreateVideoCommand : IRequest<Result<VideoCreationResponse>>
    {
        public string Name { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        //public DateTimeOffset DateOfCreation { get; set; } = DateTimeOffset.Now;
        public string? Description { get; set; }
        public Guid? SectionId { get; set; }
    }
}
