using Application.Features.Exams.DTOs;
using Application.Common;
using MediatR;

namespace Application.Features.Exams.Query.GetInstructorExams
{
    public class GetInstructorExamsQuery : IRequest<Result<InstructorExamsResult>>
    {
        public GetInstructorExamsRequest Request { get; set; } = new ();
    }
}
