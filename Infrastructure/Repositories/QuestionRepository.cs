using Microsoft.EntityFrameworkCore;
using Application.DTOs.Questions;
using Application.DTOs.Answer;
using Application.Interfaces;
using Infrastructure.Data;
using Domain.Entities;
using Domain.enums;

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
                    SectionId = q.SectionId ?? Guid.Empty,
                    CourseId = q.CourseId,
                    QuestionString = q.QuestionString,
                    QuestionImageUrl = q.QuestionImageUrl,
                    CreatedAt = q.CreatedAt,
                    UpdatedAt = q.UpdatedAt ?? q.CreatedAt,
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

        public async Task<IEnumerable<AllQuestionsInExamResponse>> GetAllQuestionsInExamAsync(Guid examId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<ExamQuestions>()
                .Where(eb => eb.ExamId == examId)
                .Select(eb => new AllQuestionsInExamResponse
                {
                    Id = eb.Question.Id,
                    QuestionString = eb.Question.QuestionString,
                    QuestionImageUrl = eb.Question.QuestionImageUrl,
                    SectionId = eb.Question.SectionId ?? Guid.Empty,
                    CourseId = eb.Question.CourseId
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<QuestionsInExamWithAnswersResponse>> GetAllQuestionsInExamWithAnswersAsync(Guid examId, CancellationToken cancellationToken = default)
        {
            var examExists = await _context.Exams
                .AnyAsync(e => e.Id == examId, cancellationToken);

            if (!examExists)
            {
                return [];
            }

            return await _context.Set<ExamQuestions>()
                .AsNoTracking()
                .Where(eb => eb.ExamId == examId)
                .Select(eb => new QuestionsInExamWithAnswersResponse
                {
                    Id = eb.Question.Id,
                    QuestionString = eb.Question.QuestionString,
                    QuestionImageUrl = eb.Question.QuestionImageUrl,
                    QuestionMark = eb.QuestionMark,
                    SectionId = eb.Question.SectionId ?? Guid.Empty,
                    CourseId = eb.Question.CourseId,
                    AllAnswersInExam = eb.Question.Answers
                                                    .Select(a => new AnswerDto
                                                    {
                                                        Id = a.Id,
                                                        AnswerString = a.AnswerText,
                                                        IsCorrect = a.IsCorrect
                                                    })
                                                    .ToList()
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<QuestionsInExamWithAnswersResponse>> GetAllQuestionsInBankWithAnswersAsync(QuestionRequest Bank, CancellationToken cancellationToken = default)
        {
            return await _context.Questions.Where(q => Bank.Type == EntityType.Course ? q.CourseId == Bank.Id :
                                                         (Bank.Type == EntityType.Section && q.SectionId == Bank.Id))
                                            .Select(q => new QuestionsInExamWithAnswersResponse
                                            {
                                                Id = q.Id,
                                                QuestionString = q.QuestionString,
                                                QuestionImageUrl = q.QuestionImageUrl,
                                                SectionId = q.SectionId ?? Guid.Empty,
                                                CourseId = q.CourseId,
                                                AllAnswersInExam = q.Answers
                                                                .Select(a => new AnswerDto
                                                                {
                                                                    Id = a.Id,
                                                                    AnswerString = a.AnswerText,
                                                                    IsCorrect = a.IsCorrect
                                                                })
                                                                .ToList()
                                            })
                                            .ToListAsync(cancellationToken);

        }
    }

}

