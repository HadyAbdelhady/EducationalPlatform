using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Centers.Commands.DeleteCenter
{
    public class DeleteCenterCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }
    }
}
