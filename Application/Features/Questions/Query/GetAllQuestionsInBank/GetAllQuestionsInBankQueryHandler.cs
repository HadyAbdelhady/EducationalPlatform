using Application.DTOs.Questions;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Questions.Query.GetAllQuestionsInBank
{
    public class GetAllQuestionsInBankQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllQuestionsInBankQuery, Result<PaginatedResult<AllQuestionsInExamResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<PaginatedResult<AllQuestionsInExamResponse>>> Handle(GetAllQuestionsInBankQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _unitOfWork.GetRepository<IQuestionRepository>()
                                                                               .GetAllQuestionsInExamAsync(request.BankId, cancellationToken);
                var responseList = response.ToList();

                if (responseList.Count == 0)
                {
                    return Result<PaginatedResult<AllQuestionsInExamResponse>>.FailureStatusCode(
                        $"No questions found for bank with ID {request.BankId}.",
                        ErrorType.NotFound);
                }

                return Result<PaginatedResult<AllQuestionsInExamResponse>>.Success(new PaginatedResult<AllQuestionsInExamResponse>
                {
                    Items = responseList,
                    PageNumber = 1,
                    PageSize = responseList.Count,
                    TotalCount = responseList.Count
                });
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<AllQuestionsInExamResponse>>.FailureStatusCode(
                    $"An error occurred while retrieving questions: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
