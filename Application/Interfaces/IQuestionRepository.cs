using Application.DTOs.Question;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IQuestionRepository : IRepository<Question>
    {
        Task<QuestionDetailsResponse?> GetQuestionByIdAsync(Guid questionId, CancellationToken cancellationToken = default);
        Task<IEnumerable<AllQuestionsInBankResponse>> GetAllQuestionsInBankAsync(Guid bankId, CancellationToken cancellationToken = default);
        Task<IEnumerable<GetAllQuestionsInExamResponse>> GetAllQuestionsInExamAsync(Guid examId, CancellationToken cancellationToken = default);
    }
}

