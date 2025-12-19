using Application.DTOs.Questions;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Questions.Query.GetAllQuestionsInExam
{
    public class GetAllQuestionsInExamQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllQuestionsInExamQuery, Result<PaginatedResult<GetAllQuestionsInExamResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<PaginatedResult<GetAllQuestionsInExamResponse>>> Handle(GetAllQuestionsInExamQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _unitOfWork.GetRepository<IQuestionRepository>().GetAllQuestionsInExamAsync(request.ExamId, cancellationToken);
                var responseList = response.ToList();

                if (responseList.Count == 0)
                {
                    return Result<PaginatedResult<GetAllQuestionsInExamResponse>>.FailureStatusCode(
                        $"No questions found for exam with ID {request.ExamId}.",
                        ErrorType.NotFound);
                }

                return Result<PaginatedResult<GetAllQuestionsInExamResponse>>.Success(new PaginatedResult<GetAllQuestionsInExamResponse>
                {
                    Items = responseList,
                    PageNumber = 1,
                    PageSize = responseList.Count,
                    TotalCount = responseList.Count
                });
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<GetAllQuestionsInExamResponse>>.FailureStatusCode(
                    $"An error occurred while retrieving exam questions: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
