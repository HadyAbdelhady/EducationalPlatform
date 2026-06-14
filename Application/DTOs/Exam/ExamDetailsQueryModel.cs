using Application.DTOs.Questions;
using Domain.enums;

namespace Application.DTOs.Exam
{
    public class ExamDetailsQueryModel
    {
        public Guid ExamId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTimeOffset? ScheduledDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public int? DurationInMinutes { get; set; }
        public ExamType ExamType { get; set; }
        public decimal TotalMark { get; set; } = 0;
        public int NumberOfQuestions { get; set; } = 0;
        public ICollection<QuestionsInExamWithAnswersResponse> AllQuestionsInExam { get; set; } = [];
    }

}