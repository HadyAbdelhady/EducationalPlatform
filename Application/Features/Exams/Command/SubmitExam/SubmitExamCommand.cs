using Application.Common;
using Application.Features.Exams.DTOs;
using MediatR;

namespace Application.Features.Exams.Command.SubmitExam
{
    public record SubmitExamRequest
    {
        public Guid ExamId { get; set; }
        public List<StudentAnswersDTO> Answers { get; set; } = [];
    }
    public record SubmitExamCommand : IRequest<Result<SubmissionResponse>>
    {
        public Guid StudentId { get; set; }
        public Guid ExamId { get; set; }
        public List<StudentAnswersDTO> Answers { get; set; } = [];
    }

    public record StudentAnswersDTO
    {
        public Guid QuestionId { get; set; }
        public Guid? ChosenAnswerId { get; set; }
    }
}
