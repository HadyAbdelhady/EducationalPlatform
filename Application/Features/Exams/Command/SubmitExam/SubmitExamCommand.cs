using Application.ResultWrapper;
using Application.DTOs.Exam;
using MediatR;

namespace Application.Features.Exams.Command.SubmitExam
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
