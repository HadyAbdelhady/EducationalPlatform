using Application.DTOs.Exam;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Exam.Command.SubmitExam
{
    public record SubmitExamCommand : IRequest<Result<SubmissionResponse>>
    {
        public Guid Student { get; set; }
        public Guid Exam { get; set; }
        public List<StudentAnswers> Answers { get; set; } = [];
    }

    public record StudentAnswers
    {
        public Guid QuestionId { get; set; }
        public Guid? ChosenAnswerId { get; set; }
    }
}
