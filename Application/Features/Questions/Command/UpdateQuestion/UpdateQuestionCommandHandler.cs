using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Questions.Command.UpdateQuestion
{
    public class UpdateQuestionHandler(IUnitOfWork unitOfWork, IQuestionUpdateService questionUpdateService) : IRequestHandler<UpdateQuestionCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IQuestionUpdateService _questionUpdateService = questionUpdateService;

        public async Task<Result<Guid>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingQuestion = await _unitOfWork.Repository<Question>()
                    .GetByIdAsync(request.QuestionId, cancellationToken);

                if (existingQuestion == null)
                {
                    return Result<Guid>.FailureStatusCode("Question not found.", ErrorType.NotFound);
                }

                // Use the service to update the question and its answers
                _questionUpdateService.UpdateQuestion(
                    existingQuestion,
                    request.QuestionString,
                    request.QuestionImageUrl,
                    request.Answers);

                _unitOfWork.Repository<Question>().Update(existingQuestion);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(existingQuestion.Id);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Result<Guid>.FailureStatusCode(ex.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<Guid>.FailureStatusCode(
                    $"Failed to update question: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}