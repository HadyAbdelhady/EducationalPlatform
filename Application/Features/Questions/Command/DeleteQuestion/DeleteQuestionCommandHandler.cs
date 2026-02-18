using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Questions.Command.DeleteQuestion
{
    public class DeleteQuestionHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<DeleteQuestionCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<Guid>> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var question = await _unitOfWork.Repository<Question>()
                    .GetByIdAsync(request.QuestionId, cancellationToken);

                if (question == null)
                {
                    return Result<Guid>.FailureStatusCode("Question not found.", ErrorType.NotFound);
                }

                var now = DateTimeOffset.UtcNow;
                question.UpdatedAt = now;

                // Optionally: also soft-delete all related answers (recommended for consistency)
                foreach (var answer in question.Answers)
                {
                    if (!answer.IsDeleted) // though global filter means they shouldn't be deleted yet
                    {
                        answer.IsDeleted = true;
                        answer.UpdatedAt = now;
                    }
                }

                await _unitOfWork.Repository<Question>().RemoveAsync(question.Id, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(question.Id);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Result<Guid>.FailureStatusCode(ex.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<Guid>.FailureStatusCode(
                    $"Error deleting question: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}