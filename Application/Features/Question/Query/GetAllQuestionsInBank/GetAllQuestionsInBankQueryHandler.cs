using Application.DTOs.Question;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Question.Query.GetAllQuestionsInBank
{
    public class GetAllQuestionsInBankQueryHandler : IRequestHandler<GetAllQuestionsInBankQuery, Result<PaginatedResult<AllQuestionsInBankResponse>>>
    {
        public Task<Result<PaginatedResult<AllQuestionsInBankResponse>>> Handle(GetAllQuestionsInBankQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
