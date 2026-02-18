using Application.DTOs.Questions;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Questions.Query.GetAllQuestionsInExam
{
    public class GetAllQuestionsInExamQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllQuestionsInExamQuery, Result<PaginatedResult<QuestionsInExamResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<PaginatedResult<QuestionsInExamResponse>>> Handle(GetAllQuestionsInExamQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _unitOfWork.GetRepository<IQuestionRepository>().GetAllQuestionsInExamAsync(request.ExamId, cancellationToken);
                var responseList = response.ToList();

                if (responseList.Count == 0)
                {
                    return Result<PaginatedResult<QuestionsInExamResponse>>.FailureStatusCode(
                        $"No questions found for exam with ID {request.ExamId}.",
                        ErrorType.NotFound);
                }

                return Result<PaginatedResult<QuestionsInExamResponse>>.Success(new PaginatedResult<QuestionsInExamResponse>
                {
                    Items = responseList,
                    PageNumber = 1,
                    PageSize = responseList.Count,
                    TotalCount = responseList.Count
                });
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<QuestionsInExamResponse>>.FailureStatusCode(
                    $"An error occurred while retrieving exam questions: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
