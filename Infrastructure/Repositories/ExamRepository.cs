using Application.DTOs.Answer;
using Application.DTOs.Exam;
using Application.DTOs.Questions;
using Application.Features.Exams.Command.UpdateExam;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ExamRepository(EducationDbContext context) : Repository<Exam>(context), IExamRepository
    {
        public async Task<ExamModelAnswer?> GetExamWithQuestionsAndAnswersByIdAsync(Guid examId, CancellationToken cancellationToken = default)
        {

            return await _context.Exams
                                  .Where(e => e.Id == examId)
                                  .Select(e => new ExamModelAnswer
                                  {
                                      ExamId = e.Id,
                                      Title = e.Name,
                                      PassMarkPercentage = e.PassMarkPercentage,
                                      TotalMark = e.TotalMark,
                                      Questions = e.ExamQuestions
                                                    .SelectMany(eq => eq.Question.Answers
                                                    .Where(a => a.IsCorrect)
                                                        .Select(a => new QuestionModelAnswer
                                                        {
                                                            QuestionId = eq.QuestionId,
                                                            CorrectAnswerId = a.Id,
                                                            QuestionMark = eq.QuestionMark
                                                        }))
                                                        .ToList()
                                  })
                                  .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ExamEditDto?> GetExamByIdWithQuestionsAndAnswersAsync(UpdateExamCommand request, CancellationToken cancellationToken = default)
        {
            if (request.ExamId == Guid.Empty)
            {
                throw new ArgumentException("ExamId cannot be empty.");
            }
            return await _context.Exams
                                 .Include(e => e.ExamQuestions!)
                                    .ThenInclude(eq => eq.Question)
                                        .ThenInclude(q => q.Answers)
                                 .Select(e => new ExamEditDto
                                 {
                                     ExamId = e.Id,
                                     Title = e.Name,
                                     Description = e.Description,
                                     ScheduledDate = e.StartTime,
                                     DurationInMinutes = e.DurationInMinutes,
                                     TotalMark = e.TotalMark,
                                     NumberOfQuestions = e.NumberOfQuestions,
                                     PassMarkPercentage = e.PassMarkPercentage,
                                     ModifiedQuestions = e.ExamQuestions
                                                        .Select(eq => new ModifiedQuestionsDto
                                                        {
                                                            Id = eq.Question.Id,
                                                            Mark = eq.QuestionMark,

                                                        })
                                                        .ToList()
                                 })
                                 .FirstOrDefaultAsync(e => e.ExamId == request.ExamId, cancellationToken);
        }

        public async Task<Exam?> GetExamEntityByIdAsync(Guid examId, CancellationToken ct)
        {
            return await _context.Exams
                .Include(e => e.ExamQuestions)
                    .ThenInclude(eq => eq.Question)
                        .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(e => e.Id == examId, ct);
        }

    }

}
