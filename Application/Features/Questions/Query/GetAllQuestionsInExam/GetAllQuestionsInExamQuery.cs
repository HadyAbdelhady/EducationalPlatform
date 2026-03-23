using Application.DTOs.Questions;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Questions.Query.GetAllQuestionsInBank
{
    public class GetAllQuestionsInExamQuery : IRequest<Result<PaginatedResult<AllQuestionsInExamResponse>>>
    {
        public Guid BankId { get; set; }
    }
}
