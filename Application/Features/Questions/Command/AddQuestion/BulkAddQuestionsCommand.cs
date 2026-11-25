using Application.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Questions.Command.AddQuestion
{
    public record BulkAddQuestionsCommand : IRequest<Result<List<Guid>>>
    {
        public Guid CourseId { get; set; }
        public Guid? SectionId { get; set; }
        public List<BulkAddQuestionItem> Questions { get; set; } = [];
    }

    public record BulkAddQuestionItem
    {
        public string QuestionString { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public string? PictureUrl { get; set; }
        public IFormFile? PictureFile { get; set; }
        public List<string> AnswerTexts { get; set; } = [];
        public List<bool> IsCorrects { get; set; } = [];
    }
}

