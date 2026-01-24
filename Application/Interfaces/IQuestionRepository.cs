using Application.DTOs.Questions;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IQuestionRepository : IRepository<Question>
    {
        Task<QuestionDetailsResponse?> GetQuestionByIdAsync(Guid questionId, CancellationToken cancellationToken = default);
        Task<IEnumerable<AllQuestionsInBankResponse>> GetAllQuestionsInBankAsync(Guid bankId, CancellationToken cancellationToken = default);
        Task<IEnumerable<QuestionsInExamResponse>> GetAllQuestionsInExamAsync(Guid examId, CancellationToken cancellationToken = default);
    }
}

