using Application.DTOs.Question;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Question.Query.GetQuestionById
{
    public class GetQuestionByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetQuestionByIdQuery, Result<QuestionDetailsResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<QuestionDetailsResponse>> Handle(GetQuestionByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _unitOfWork.GetRepository<IQuestionRepository>().GetQuestionByIdAsync(request.QuestionId, cancellationToken);

                if (response == null)
                {
                    return Result<QuestionDetailsResponse>.FailureStatusCode(
                        $"Question with ID {request.QuestionId} not found.", 
                        ErrorType.NotFound);
                }

                return Result<QuestionDetailsResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<QuestionDetailsResponse>.FailureStatusCode(
                    $"An error occurred while retrieving the question: {ex.Message}", 
                    ErrorType.InternalServerError);
            }
        }
    }
}
