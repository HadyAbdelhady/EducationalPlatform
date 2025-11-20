using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Question.Command.AddQuestion
{
    public class AddQuestionHandler(IUnitOfWork context) : IRequestHandler<AddQuestionCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = context;

        public async Task<Result<Guid>> Handle(AddQuestionCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                // 1. Create the Question Entity
                var question = new Domain.Entities.Question
                {
                    Id = Guid.NewGuid(),
                    QuestionString = request.QuestionString,
                    QuestionImageUrl = request.QuestionImageUrl,
                    QuestionMark = request.Mark,
                };

                // 2. Create the Answer Entities
                foreach (var answerDto in request.Answers)
                {
                    var answer = new Answer
                    {
                        Id = Guid.NewGuid(),
                        QuestionId = question.Id, // Link to parent
                        AnswerText = answerDto.AnswerText,
                        IsCorrect = answerDto.IsCorrect,
                    };
                    question.Answers.Add(answer);
                }
                await _unitOfWork.Repository<Domain.Entities.Question>().AddAsync(question, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<Guid>.Success(question.Id);
            }
            catch (UnauthorizedAccessException auth)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<Guid>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<Guid>.FailureStatusCode($"Error creating Question: {ex.Message}", ErrorType.Conflict);
            }
        }
    }
}
