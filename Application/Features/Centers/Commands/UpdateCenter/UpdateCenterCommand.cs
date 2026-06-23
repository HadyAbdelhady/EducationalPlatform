using Application.DTOs.Center;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Centers.Commands.UpdateCenter
{
    public class UpdateCenterCommand : IRequest<Result<CenterResponse>>
    {
        public Guid Id { get; set; }
        public UpdateCenterRequest Request { get; set; } = new();
    }
}
