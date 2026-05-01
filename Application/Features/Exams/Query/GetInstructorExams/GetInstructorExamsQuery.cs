using Application.DTOs.Exam;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Exams.Query.GetInstructorExams
{
    public class GetInstructorExamsQuery : IRequest<Result<InstructorExamsResult>>
    {
        public GetInstructorExamsRequest Request { get; set; } = new ();
    }
}
