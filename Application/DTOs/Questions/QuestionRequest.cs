using Domain.enums;

namespace Application.DTOs.Questions
{
    public record QuestionRequest
    {
        public int PageNumber { get; set; } = 1;
        public EntityType Type { get; set; }
        public Guid Id { get; set; }
    }
}
