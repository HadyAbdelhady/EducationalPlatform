using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Centers.Commands.RemoveInstructorFromCenter
{
    public class RemoveInstructorFromCenterCommand : IRequest<Result<bool>>
    {
        public Guid CenterId { get; set; }
        public Guid InstructorId { get; set; }
    }
}
