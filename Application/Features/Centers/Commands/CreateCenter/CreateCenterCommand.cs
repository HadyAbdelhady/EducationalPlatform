using Application.DTOs.Center;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Centers.Commands.CreateCenter
{
    public class CreateCenterCommand : IRequest<Result<CenterResponse>>
    {
        public CreateCenterRequest Request { get; set; } = new();
    }
}
