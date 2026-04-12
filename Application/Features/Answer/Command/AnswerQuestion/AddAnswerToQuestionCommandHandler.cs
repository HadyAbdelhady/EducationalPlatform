using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Answer.Command.AnswerQuestion
{
    public class AddAnswerToQuestionCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddAnswerToQuestionCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<Guid>> Handle(AddAnswerToQuestionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var question = await _unitOfWork.Repository<Question>()
                    .GetByIdAsync(request.QuestionId, cancellationToken);

                if (question == null)
                {
                    return Result<Guid>.FailureStatusCode("Question not found.", ErrorType.NotFound);
                }

                var answer = new Answer
                {
                    Id = Guid.NewGuid(),
                    QuestionId = question.Id,
                    AnswerText = request.AnswerText,
                    IsCorrect = request.IsCorrect,
                    Explanation = request.Explanation,
                    CreatedAt = DateTimeOffset.UtcNow,
                    IsDeleted = false
                };

                question.Answers.Add(answer);
                question.UpdatedAt = DateTimeOffset.UtcNow;

                _unitOfWork.Repository<Question>().Update(question);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(answer.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.FailureStatusCode(
                    $"Failed to add answer: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
