using Application.DTOs.Answer;
using Application.ResultWrapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Application.Features.Questions.Command.UpdateQuestion
{
    public record UpdateQuestionCommand : IRequest<Result<Guid>>
    {
        public Guid QuestionId { get; set; }
        public string QuestionString { get; set; } = string.Empty;
        public string? QuestionImageUrl { get; set; }
        public decimal? Mark { get; set; }
        public List<UpdateAnswerDto> Answers { get; set; } = new();

        [JsonIgnore]
        public IFormFile? PictureFile { get; set; }
    }
}