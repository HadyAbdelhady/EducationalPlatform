using Application.Common.Interfaces;
using Application.Features.Questions.DTOs;

using Domain.Entities;

namespace Application.Features.Questions.Interfaces
{
    public interface IQuestionRepository : IRepository<Question>
    {
        Task<QuestionDetailsResponse?> GetQuestionByIdAsync(Guid questionId, CancellationToken cancellationToken = default);
        Task<IEnumerable<AllQuestionsInExamResponse>> GetAllQuestionsInExamAsync(Guid bankId, CancellationToken cancellationToken = default);
        Task<IEnumerable<QuestionsInExamWithAnswersResponse>> GetAllQuestionsInExamWithAnswersAsync(Guid examId, CancellationToken cancellationToken = default);
        Task<IEnumerable<QuestionsInExamWithAnswersResponse>> GetAllQuestionsInBankWithAnswersAsync(QuestionRequest Bank, CancellationToken cancellationToken = default);
    }
}

