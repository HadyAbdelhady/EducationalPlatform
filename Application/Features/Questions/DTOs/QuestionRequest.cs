using Domain.enums;

namespace Application.Features.Questions.DTOs
{
    public record QuestionRequest
    {
        public int PageNumber { get; set; } = 1;
        public EntityType Type { get; set; }
        public Guid Id { get; set; }
    }
}
