using Application.DTOs.Exam;
using Application.Features.Exams.Command.UpdateExam;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IExamRepository : IRepository<Exam>
    {
        Task<ExamEditDto?> GetExamByIdWithQuestionsAndAnswersAsync(UpdateExamCommand request, CancellationToken cancellationToken = default);
        Task<ExamDetailsQueryModel?> GetExamByIdWithQuestionsAndAnswersAsync(Guid ExamId, CancellationToken cancellationToken = default);
        Task<Exam?> GetExamEntityByIdAsync(Guid examId, CancellationToken ct);
        public Task<ExamModelAnswer?> GetExamWithQuestionsAndAnswersByIdAsync(Guid examId, CancellationToken cancellationToken = default);
    }
}
