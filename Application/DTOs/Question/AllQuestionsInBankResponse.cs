namespace Application.DTOs.Question
{
    public class AllQuestionsInBankResponse
    {
        public int Id { get; set; }
        public string QuestionString { get; set; } = string.Empty;
        public string? QuestionImageUrl { get; set; }
        public decimal? QuestionMark { get; set; }
    }
}
