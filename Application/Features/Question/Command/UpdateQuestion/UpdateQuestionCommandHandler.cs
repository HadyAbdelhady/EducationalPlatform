using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Question.Command.UpdateQuestion
{
    public class UpdateQuestionHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateQuestionCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<Guid>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingQuestion = await _unitOfWork.Repository<Domain.Entities.Question>()
                    .GetByIdAsync(request.QuestionId, cancellationToken);

                if (existingQuestion == null)
                {
                    return Result<Guid>.FailureStatusCode("Question not found.", ErrorType.NotFound);
                }

                // Update question
                existingQuestion.QuestionString = request.QuestionString;
                existingQuestion.QuestionImageUrl = request.QuestionImageUrl;
                existingQuestion.UpdatedAt = DateTimeOffset.UtcNow;

                var now = DateTimeOffset.UtcNow;
                var currentAnswers = existingQuestion.Answers.ToList(); // All are non-deleted (thanks to global filter)
                var incomingAnswerIds = new HashSet<Guid>();

                foreach (var dto in request.Answers)
                {
                    if (dto.Id is not null)
                    {
                        var existingAnswer = currentAnswers.FirstOrDefault(a => a.Id == dto.Id);
                        if (existingAnswer != null)
                        {
                            existingAnswer.AnswerText = dto.AnswerText;
                            existingAnswer.IsCorrect = dto.IsCorrect;
                            existingAnswer.UpdatedAt = now;
                            incomingAnswerIds.Add(dto.Id.Value);
                        }

                    }
                    else
                    {
                        // New answer
                        existingQuestion.Answers.Add(new Answer
                        {
                            Id = Guid.NewGuid(),
                            QuestionId = existingQuestion.Id,
                            AnswerText = dto.AnswerText,
                            IsCorrect = dto.IsCorrect,
                            Explanation = dto.Explanation,
                            CreatedAt = now,
                            UpdatedAt = now,
                            IsDeleted = false
                        });
                    }
                }

                // Soft-delete answers not in the update (they'll be hidden by global filter afterward)
                foreach (var answer in currentAnswers)
                {
                    if (!incomingAnswerIds.Contains(answer.Id))
                    {
                        answer.IsDeleted = true;
                        answer.UpdatedAt = now;
                    }
                }

                _unitOfWork.Repository<Domain.Entities.Question>().Update(existingQuestion);
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