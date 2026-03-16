using Domain.enums;

namespace Application.DTOs.Questions
{
    public record QuestionRequest
    {
        public EntityType Type { get; set; }
        public Guid Id { get; set; }
    }
}
