using Application.DTOs.Question;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class QuestionRepository(EducationDbContext context) : Repository<Question>(context), IQuestionRepository
    {
        public async Task<QuestionDetailsResponse?> GetQuestionByIdAsync(Guid questionId, CancellationToken cancellationToken = default)
        {
            return await _context.Questions
                .Where(q => q.Id == questionId)
                .Select(q => new QuestionDetailsResponse
                {
                    Id = q.Id,
                    SectionId = q.SectionId,
                    CourseId = q.CourseId,
                    QuestionString = q.QuestionString,
                    QuestionImageUrl = q.QuestionImageUrl,
                    CreatedAt = q.CreatedAt,
                    UpdatedAt = q.UpdatedAt,
                    Answers = q.Answers
                        .Where(a => !a.IsDeleted)
                        .Select(a => new AnswerResponse
                        {
                            Id = a.Id,
                            AnswerText = a.AnswerText,
                            IsCorrect = a.IsCorrect
                        })
                        .ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<AllQuestionsInBankResponse>> GetAllQuestionsInBankAsync(Guid examId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<ExamBank>()
                .Where(eb => eb.ExamId == examId)
                .Select(eb => new AllQuestionsInBankResponse
                {
                    Id = eb.Question.Id,
                    QuestionString = eb.Question.QuestionString,
                    QuestionImageUrl = eb.Question.QuestionImageUrl,
                    SectionId = eb.Question.SectionId,
                    CourseId = eb.Question.CourseId
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<GetAllQuestionsInExamResponse>> GetAllQuestionsInExamAsync(Guid examId, CancellationToken cancellationToken = default)
        {
            // First check if exam exists
            var examExists = await _context.Exams
                .AnyAsync(e => e.Id == examId, cancellationToken);

            if (!examExists)
            {
                return [];
            }

            // Single query with join to get all questions for the exam
            return await _context.Set<ExamBank>()
                .Where(eb => eb.ExamId == examId)
                .Select(eb => new GetAllQuestionsInExamResponse
                {
                    Id = eb.Question.Id,
                    QuestionString = eb.Question.QuestionString,
                    QuestionImageUrl = eb.Question.QuestionImageUrl,
                    QuestionMark = (decimal)eb.QuestionMark,
                    SectionId = eb.Question.SectionId,
                    CourseId = eb.Question.CourseId
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}

