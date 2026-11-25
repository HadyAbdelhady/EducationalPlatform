using Application.Common;
using Application.Features.Answers.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Questions.Command.UpdateQuestion
{
    public record BulkUpdateQuestionsCommand : IRequest<Result<List<Guid>>>
    {
        public List<BulkUpdateQuestionItem> Questions { get; set; } = [];
    }

    public record BulkUpdateQuestionItem
    {
        public Guid QuestionId { get; set; }
        public string QuestionString { get; set; } = string.Empty;
        public string? QuestionImageUrl { get; set; }
        public decimal? Mark { get; set; }
        public IFormFile? PictureFile { get; set; }
        public List<UpdateAnswerDto> Answers { get; set; } = [];
    }
}

