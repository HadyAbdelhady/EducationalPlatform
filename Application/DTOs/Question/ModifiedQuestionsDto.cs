using Application.DTOs.Answer;

namespace Application.DTOs.Question
{
    public class ModifiedQuestionsDto
    {
        public Guid Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Mark { get; set; }
        public ICollection<UpdateAnswerDto> Answers { get; set; } = [];
    }

}
