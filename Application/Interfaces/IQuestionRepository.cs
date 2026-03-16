using Application.DTOs.Questions;

using Domain.Entities;

namespace Application.Interfaces
{
    public interface IQuestionRepository : IRepository<Question>
    {
        Task<QuestionDetailsResponse?> GetQuestionByIdAsync(Guid questionId, CancellationToken cancellationToken = default);
        Task<IEnumerable<AllQuestionsInExamResponse>> GetAllQuestionsInExamAsync(Guid bankId, CancellationToken cancellationToken = default);
        Task<IEnumerable<QuestionsInExamWithAnswersResponse>> GetAllQuestionsInExamWithAnswersAsync(Guid examId, CancellationToken cancellationToken = default);
        Task<IEnumerable<QuestionsInExamWithAnswersResponse>> GetAllQuestionsInBankWithAnswersAsync(QuestionRequest Bank, CancellationToken cancellationToken = default);
    }
}

