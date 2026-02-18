using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Videos.Commands.DeleteVideo
{
    public class DeleteVideoCommand : IRequest<Result<string>>
    {
        public Guid VideoId { get; set; }
    }
}
