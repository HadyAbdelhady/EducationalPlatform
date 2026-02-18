using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Questions.Command.AddQuestion
{
    public class AddQuestionHandler(IUnitOfWork context) : IRequestHandler<AddQuestionCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = context;

        public async Task<Result<Guid>> Handle(AddQuestionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Create the Question Entity
                var question = new Question
                {
                    Id = Guid.NewGuid(),
                    QuestionString = request.QuestionString,
                    QuestionImageUrl = request.QuestionImageUrl,
                    SectionId = request.SectionId,
                    CourseId = request.CourseId,
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
                        Explanation = request.Explanation
                    };
                    question.Answers.Add(answer);
                }
                await _unitOfWork.Repository<Question>().AddAsync(question, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(question.Id);
            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<Guid>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<Guid>.FailureStatusCode($"Error creating Question: {ex.Message}", ErrorType.Conflict);
            }
        }
    }
}
