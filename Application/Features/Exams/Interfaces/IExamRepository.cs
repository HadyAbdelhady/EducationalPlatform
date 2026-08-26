using Application.Common.Interfaces;
using Application.Features.Exams.DTOs;
using Application.Features.Exams.Command.UpdateExam;
using Domain.Entities;

namespace Application.Features.Exams.Interfaces
{
    public interface IExamRepository : IRepository<Exam>
    {

        Task<ExamDetailsQueryModel?> GetExamByIdWithQuestionsAndAnswersAsync(Guid ExamId, CancellationToken cancellationToken = default);
        Task<Exam?> GetExamEntityByIdAsync(Guid examId, CancellationToken ct);
        Task<ExamModelAnswer?> GetExamWithQuestionsAndAnswersByIdAsync(Guid examId, CancellationToken cancellationToken = default);
        Task<CoursesSectionsHashMap> GetInstructorCoursesSectionsHashMapAsync(Guid instructorId, CancellationToken cancellationToken);
        IQueryable<InstructorExamsResponseDto> GetInstructorNonRandomExamsQuery(Guid instructorId);
    }
}
