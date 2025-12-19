using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Exam.Command.GenerateExam
{
    public class GenerateExamCommand : IRequest<Result<string>>
    {
        public Guid CourseId { get; set; }
        public Guid? SectionId { get; set; }
        public Guid CreatedBy { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int NumberOfQuestions { get; set; } = 0;
        public int ExamTotalMark { get; set; }
        public int PassMarkPercentage { get; set; }

        public DateTimeOffset ExamStartTime { get; set; }
        public DateTimeOffset ExamEndTime { get; set; }
        public int DurationInMinutes { get; set; }

        public bool IsRandomized { get; set; }
        public ExamType ExamType { get; set; }

        public Dictionary<Guid, decimal>? QuestionIdsWithMarks { get; set; }

    }
}
