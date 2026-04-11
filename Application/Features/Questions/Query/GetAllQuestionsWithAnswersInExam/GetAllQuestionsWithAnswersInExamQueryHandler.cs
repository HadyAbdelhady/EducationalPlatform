using Application.DTOs.Questions;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Questions.Query.GetAllQuestionsInExam
{
    public class GetAllQuestionsWithAnswersInExamQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllQuestionsWithAnswersInExamQuery, Result<PaginatedResult<QuestionsInExamWithAnswersResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<PaginatedResult<QuestionsInExamWithAnswersResponse>>> Handle(GetAllQuestionsWithAnswersInExamQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _unitOfWork.GetRepository<IQuestionRepository>().GetAllQuestionsInExamWithAnswersAsync(request.ExamId, cancellationToken);
                var responseList = response.ToList();

                if (responseList.Count == 0)
                {
                    return Result<PaginatedResult<QuestionsInExamWithAnswersResponse>>.FailureStatusCode(
                        $"No questions found for exam with ID {request.ExamId}.",
                        ErrorType.NotFound);
                }

                return Result<PaginatedResult<QuestionsInExamWithAnswersResponse>>.Success(new PaginatedResult<QuestionsInExamWithAnswersResponse>
                {
                    Items = responseList,
                    PageNumber = 1,
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
