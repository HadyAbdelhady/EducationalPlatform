using Application.Features.Answers.DTOs;
using Domain.Entities;

namespace Application.Features.Questions.Interfaces
{
    public interface IQuestionUpdateService
    {
        void UpdateQuestion(Question question, string questionString, string? questionImageUrl, List<UpdateAnswerDto> answers);
    }
}

