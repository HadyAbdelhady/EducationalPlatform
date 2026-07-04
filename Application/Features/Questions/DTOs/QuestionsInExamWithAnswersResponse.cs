using Application.Features.Answers.DTOs;

namespace Application.Features.Questions.DTOs
{
    public class QuestionsInExamWithAnswersResponse
    {
        public Guid Id { get; set; }
        public string QuestionString { get; set; } = string.Empty;
        public string? QuestionImageUrl { get; set; }
        public decimal? QuestionMark { get; set; }
        public Guid SectionId { get; set; }
        public Guid CourseId { get; set; }
        public ICollection<AnswerDto> AllAnswersInExam { get; set; } = [];

    }
}

