using Application.DTOs.Exam;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IExamRepository : IRepository<Exam>
    {
        public Task<ExamModelAnswer?> GetExamWithQuestionsAndAnswersByIdAsync(Guid examId, CancellationToken cancellationToken = default);
    }
}
