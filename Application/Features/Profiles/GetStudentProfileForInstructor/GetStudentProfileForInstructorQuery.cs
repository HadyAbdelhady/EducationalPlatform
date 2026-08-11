using Application.Common;
using Application.Features.Profiles.DTOs;
using MediatR;

namespace Application.Features.Profiles.GetStudentProfileForInstructor
{
    public class GetStudentProfileForInstructorQuery : IRequest<Result<StudentProfileForInstructorResponse>>
    {
        public Guid InstructorId { get; set; }
        public Guid StudentId { get; set; }
    }
}
