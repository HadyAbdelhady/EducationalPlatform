using Application.DTOs.Question;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Question.Query.GetAllQuestionsInBank
{
    public class GetAllQuestionsInBankQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllQuestionsInBankQuery, Result<PaginatedResult<AllQuestionsInBankResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<PaginatedResult<AllQuestionsInBankResponse>>> Handle(GetAllQuestionsInBankQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _unitOfWork.GetRepository<IQuestionRepository>().GetAllQuestionsInBankAsync(request.BankId, cancellationToken);
                var responseList = response.ToList();

                if (responseList.Count == 0)
                {
                    return Result<PaginatedResult<AllQuestionsInBankResponse>>.FailureStatusCode(
                        $"No questions found for bank with ID {request.BankId}.",
                        ErrorType.NotFound);
                }

                return Result<PaginatedResult<AllQuestionsInBankResponse>>.Success(new PaginatedResult<AllQuestionsInBankResponse>
                {
                    Items = responseList,
                    PageNumber = 1,
                    PageSize = responseList.Count,
                    TotalCount = responseList.Count
                });
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<AllQuestionsInBankResponse>>.FailureStatusCode(
                    $"An error occurred while retrieving questions: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
