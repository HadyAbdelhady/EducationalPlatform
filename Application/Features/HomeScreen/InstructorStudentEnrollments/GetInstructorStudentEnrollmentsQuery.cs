using Application.Features.HomeScreen.DTOs;
using Application.Common;
using MediatR;

namespace Application.Features.HomeScreen.InstructorStudentEnrollments
{
    public class GetInstructorStudentEnrollmentsQuery : IRequest<Result<InstructorStudentEnrollmentsResponse>>
    {
        public Guid InstructorId { get; set; }

        public Guid StudentId { get; set; }
    }
}
