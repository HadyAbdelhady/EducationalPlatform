using Application.DTOs.Answer;
using Application.ResultWrapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Questions.Command.AddQuestion
{
    public record AddQuestionCommand : IRequest<Result<Guid>>
    {
        public string QuestionString { get; init; } = string.Empty;
        public string? QuestionImageUrl { get; init; }
        public Guid? SectionId { get; set; }
        public Guid CourseId { get; set; }
        public string Explanation { get; init; } = string.Empty;
        public string? PictureUrl { get; set; }
        public IFormFile? PictureFile { get; set; }


        public required List<string> AnswerTexts { get; set; } 
        public required List<bool> IsCorrects { get; set; } 
        //public required List<CreateAnswerDto> Answers { get; set; }
    }
}
