using Application.DTOs.Answer;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Questions.Command.AddQuestion
{
    public record AddQuestionCommand : IRequest<Result<Guid>>
    {
        public string QuestionString { get; init; } = string.Empty;
        public string? QuestionImageUrl { get; init; }
        public decimal Mark { get; init; }
        public Guid SectionId { get; set; }
        public Guid CourseId { get; set; }
        public string Explanation { get; init; } = string.Empty;

        public List<CreateAnswerDto> Answers { get; init; } = [];
    }
}
