using Application.DTOs.Questions;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Questions.Query.GetAllQuestionsWithAnswersInBank
{
    public class GetAllQuestionsWithAnswersInBankQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllQuestionsWithAnswersInBankQuery, Result<PaginatedResult<QuestionsInExamWithAnswersResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<PaginatedResult<QuestionsInExamWithAnswersResponse>>> Handle(GetAllQuestionsWithAnswersInBankQuery request, CancellationToken cancellationToken)
        {
            try
            {
                QuestionRequest questionRequest = new()
                {
                    Id = request.BankId,
                    Type = request.BankType
                };
                var response = await _unitOfWork.GetRepository<IQuestionRepository>().GetAllQuestionsInBankWithAnswersAsync(questionRequest, cancellationToken);
                var responseList = response.ToList();

                if (responseList.Count == 0)
                {
                    return Result<PaginatedResult<QuestionsInExamWithAnswersResponse>>.FailureStatusCode(
                        $"No questions found for exam with ID {questionRequest.Id}.",
                        ErrorType.NotFound);
                }

                return Result<PaginatedResult<QuestionsInExamWithAnswersResponse>>.Success(new PaginatedResult<QuestionsInExamWithAnswersResponse>
                {
                    Items = [.. responseList.Skip(request.PageNumber - 1).Take(responseList.Count)],
                    PageNumber = questionRequest.PageNumber,
                    PageSize = responseList.Count,
                    TotalCount = responseList.Count
                });
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<QuestionsInExamWithAnswersResponse>>.FailureStatusCode(
                    $"An error occurred while retrieving exam questions: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
