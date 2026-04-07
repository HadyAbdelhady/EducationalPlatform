using Application.DTOs.Questions;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Questions.Query.GetAllQuestionsWithAnswersInBank
{
    public class GetAllQuestionsWithAnswersInBankQuery : IRequest<Result<PaginatedResult<QuestionsInExamWithAnswersResponse>>>
    {
        public Guid BankId { get; set; }
        public EntityType BankType { get; set; }
        public int PageNumber { get; set; } = 1;

    }
}
