using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Exam.Command.DeleteExam
{
    public class DeleteExamCommand : IRequest<Result<string>>
    {
        public Guid ExamId { get; set; }
    }
}
