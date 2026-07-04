using Application.Features.Centers.DTOs;
using Application.Common;
using MediatR;

namespace Application.Features.Centers.Commands.CreateCenter
{
    public class CreateCenterCommand : IRequest<Result<CenterResponse>>
    {
        public CreateCenterRequest Request { get; set; } = new();
    }
}
