using Application.DTOs.Answer;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Question.Command.AddQuestion
{
    public record AddQuestionCommand : IRequest<Result<Guid>>
    {
        public string QuestionString { get; init; } = string.Empty;
        public string? QuestionImageUrl { get; init; }
        public decimal Mark { get; init; } // Default mark for this question
        public Guid SectionId { get; set; }
        public Guid CourseId { get; set; }
        public List<CreateAnswerDto> Answers { get; init; } = [];
    }
}
