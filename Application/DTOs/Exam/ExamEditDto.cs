using Application.DTOs.Answer;
using Application.DTOs.Questions;

namespace Application.DTOs.Exam
{
    public class ExamEditDto
    {
        public Guid ExamId { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public DateTimeOffset? ScheduledDate { get; set; }
        public int? DurationInMinutes { get; set; }
        public decimal? TotalMark { get; set; } = 0;
        public int? NumberOfQuestions { get; set; } = 0;
        public int? PassMarkPercentage { get; set; } = 0;

        public ICollection<UpdateAnswerDto> ModifiedAnswerDto { get; set; } = [];
        public ICollection<ModifiedQuestionsDto> ModifiedQuestions { get; set; } = [];


    }
}
