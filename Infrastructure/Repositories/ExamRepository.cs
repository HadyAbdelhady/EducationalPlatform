using Application.DTOs.Exam;
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
    }

}
