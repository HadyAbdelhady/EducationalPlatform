using Application.Features.Questions.DTOs;
using Application.Common;
using MediatR;

namespace Application.Features.Questions.Query.GetAllQuestionsInExam
{
    public class GetAllQuestionsWithAnswersInExamQuery : IRequest<Result<PaginatedResult<QuestionsInExamWithAnswersResponse>>>
    {
        public Guid ExamId { get; set; }
    }
}
