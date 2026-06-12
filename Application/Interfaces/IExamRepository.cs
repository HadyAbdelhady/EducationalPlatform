using Application.DTOs.Exam;
using Application.Features.Exams.Command.UpdateExam;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IExamRepository : IRepository<Exam>
    {

        Task<ExamDetailsQueryModel?> GetExamByIdWithQuestionsAndAnswersAsync(Guid ExamId, CancellationToken cancellationToken = default);
        Task<Exam?> GetExamEntityByIdAsync(Guid examId, CancellationToken ct);
        Task<ExamModelAnswer?> GetExamWithQuestionsAndAnswersByIdAsync(Guid examId, CancellationToken cancellationToken = default);
        Task<Dictionary<Guid, Dictionary<Guid, string>>> GetInstructorCoursesSectionsHashMapAsync(Guid instructorId, CancellationToken cancellationToken);
        Task<IQueryable<InstructorExamsResponseDto>> GetInstructorNonRandomExamsQuery(Guid instructorId, CancellationToken cancellationToken);
    }
}
