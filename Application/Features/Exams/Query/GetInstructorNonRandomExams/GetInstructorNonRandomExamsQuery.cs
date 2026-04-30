using Application.DTOs.Exam;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Exams.Query.GetInstructorNonRandomExams
{
    public class GetInstructorNonRandomExamsQuery : IRequest<Result<InstructorNonRandomExamsResult>>
    {
        public GetInstructorNonRandomExamsRequest Request { get; set; } = new GetInstructorNonRandomExamsRequest();
    }
}
