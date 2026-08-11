using Application.Common;
using Application.Features.Profiles.DTOs;
using MediatR;

namespace Application.Features.Profiles.GetInstructorProfileForStudent
{
    public class GetInstructorProfileForStudentQuery : IRequest<Result<InstructorProfileForStudentResponse>>
    {
        public Guid StudentId { get; set; }
        public Guid InstructorId { get; set; }
    }
}
