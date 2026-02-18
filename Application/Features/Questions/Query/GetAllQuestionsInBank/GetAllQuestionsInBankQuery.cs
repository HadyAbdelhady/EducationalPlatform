using Application.DTOs.Questions;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Questions.Query.GetAllQuestionsInBank
{
    public class GetAllQuestionsInBankQuery : IRequest<Result<PaginatedResult<AllQuestionsInBankResponse>>>
    {
        public Guid BankId { get; set; }
    }
}
