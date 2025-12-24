using Application.DTOs.Answer;
using Application.Interfaces;
using Domain.Entities;

namespace Infrastructure.Services
{
    public class QuestionUpdateService : IQuestionUpdateService
    {
        public void UpdateQuestion(Question question, string questionString, string? questionImageUrl, List<UpdateAnswerDto> answers)
        {
            // Update question properties
            question.QuestionString = questionString;
            question.QuestionImageUrl = questionImageUrl;
            question.UpdatedAt = DateTimeOffset.UtcNow;

            var now = DateTimeOffset.UtcNow;
            var currentAnswers = question.Answers.ToList(); // All are non-deleted (thanks to global filter)
            var incomingAnswerIds = new HashSet<Guid>();

            // Process incoming answers
            foreach (var dto in answers)
            {
                if (dto.Id is not null)
                {
                    // Update existing answer
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
                    // Add new answer
                    question.Answers.Add(new Answer
                    {
                        Id = Guid.NewGuid(),
                        QuestionId = question.Id,
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
        }
    }
}

