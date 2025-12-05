using Application.ResultWrapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Videos.Commands.DeleteVideo
{
    public class DeleteVideoCommand : IRequest<Result<string>>
    {
        public Guid VideoId { get; set; }
    }
}
