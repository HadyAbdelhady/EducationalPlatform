using Application.DTOs.Answer;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IQuestionUpdateService
    {
        void UpdateQuestion(Question question, string questionString, string? questionImageUrl, List<UpdateAnswerDto> answers);
    }
}

