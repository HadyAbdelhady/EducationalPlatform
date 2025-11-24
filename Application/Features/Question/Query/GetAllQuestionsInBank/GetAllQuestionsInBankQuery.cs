using Application.DTOs.Question;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Question.Query.GetAllQuestionsInBank
{
    public class GetAllQuestionsInBankQuery : IRequest<Result<PaginatedResult<AllQuestionsInBankResponse>>>
    {
        public Guid BankId { get; set; }
    }
}
