using Application.Features.Exams.Command.UpdateExam;
using Microsoft.EntityFrameworkCore;
using Application.DTOs.Questions;
using Application.DTOs.Answer;
using Application.Interfaces;
using Application.DTOs.Exam;
using Infrastructure.Data;
using Domain.Entities;

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
                                 .Select(e => new ExamEditDto
                                 {
                                     ExamId = e.Id,
                                     Title = e.Name,
                                     Description = e.Description,
                                     ScheduledDate = e.StartTime,
                                     DurationInMinutes = e.DurationInMinutes,
                                     NumberOfQuestions = e.NumberOfQuestions,
                                     TotalMark = e.TotalMark,
                                     PassMarkPercentage = e.PassMarkPercentage,
                                     ModifiedAnswerDto = e.ExamQuestions.Where(eq => eq.ExamId == request.ExamId)
                                                                        .SelectMany(eq => eq.Question.Answers
                                                                        .Select(a => new UpdateAnswerDto
                                                                        {
                                                                            Id = a.Id,
                                                                            AnswerText = a.AnswerText,
                                                                            IsCorrect = a.IsCorrect
                                                                        }))
                                                                        .ToList(),
                                     ModifiedQuestions = e.ExamQuestions.Select(eq => new ModifiedQuestionsDto
                                     {
                                         Id = eq.Question.Id,
                                         Mark = eq.QuestionMark

                                     }).ToList()
                                 })
                                 .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ExamDetailsQueryModel?> GetExamByIdWithQuestionsAndAnswersAsync(Guid ExamId, CancellationToken cancellationToken = default)
        {
            if (ExamId == Guid.Empty)
            {
                throw new ArgumentException("ExamId cannot be empty.");
            }
            return await _context.Exams
                                 .Select(e => new ExamDetailsQueryModel
                                 {
                                     ExamId = e.Id,
                                     Title = e.Name,
                                     Description = e.Description,
                                     ScheduledDate = e.StartTime,
                                     DurationInMinutes = e.DurationInMinutes,
                                     NumberOfQuestions = e.NumberOfQuestions,
                                     TotalMark = e.TotalMark,
                                     AllQuestionsInExam = e.ExamQuestions.Where(ex => ex.ExamId == ExamId)
                                                                         .Select(eq => new QuestionsInExamResponse
                                                                         {
                                                                             Id = eq.Question.Id,
                                                                             QuestionMark = eq.QuestionMark,
                                                                             CourseId = eq.Question.CourseId,
                                                                             QuestionImageUrl = eq.Question.QuestionImageUrl,
                                                                             QuestionString = eq.Question.QuestionString,
                                                                             SectionId = eq.Question.SectionId
                                                                         }).ToList()
                                 })
                                 .FirstOrDefaultAsync(cancellationToken);
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
