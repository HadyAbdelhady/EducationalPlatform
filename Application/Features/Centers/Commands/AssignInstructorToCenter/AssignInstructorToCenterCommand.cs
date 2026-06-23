using Application.DTOs.Center;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Centers.Commands.AssignInstructorToCenter
{
    public class AssignInstructorToCenterCommand : IRequest<Result<CenterResponse>>
    {
        public Guid CenterId { get; set; }
        public AssignInstructorToCenterRequest Request { get; set; } = new();
    }
}
