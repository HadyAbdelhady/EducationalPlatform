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
                                                                         .Select(eq => new QuestionsInExamWithAnswersResponse
                                                                         {
                                                                             Id = eq.Question.Id,
                                                                             QuestionMark = eq.QuestionMark,
                                                                             CourseId = eq.Question.CourseId,
                                                                             QuestionImageUrl = eq.Question.QuestionImageUrl,
                                                                             QuestionString = eq.Question.QuestionString,
                                                                             SectionId = eq.Question.SectionId?? Guid.Empty,
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

        public async Task<Dictionary<Guid, Dictionary<Guid, string>>> GetInstructorCoursesSectionsHashMapAsync(Guid instructorId, CancellationToken cancellationToken)
        {
            var courses = await _context.Courses
                .Where(c => c.InstructorCourses.Any(ic => ic.InstructorId == instructorId))
                .Include(c => c.Sections)
                .ToListAsync(cancellationToken);

            var hashMap = new Dictionary<Guid, Dictionary<Guid, string>>();

            foreach (var course in courses)
            {
                hashMap[course.Id] = new Dictionary<Guid, string>();
                
                foreach (var section in course.Sections)
                {
                    hashMap[course.Id][section.Id] = section.Name;
                }
            }

            return hashMap;
        }

        public async Task<IQueryable<InstructorNonRandomExamsResponseDto>> GetInstructorNonRandomExamsQuery(Guid instructorId, CancellationToken cancellationToken)
        {
            return await Task.FromResult(_context.Exams
                .Where(e => e.InstructorId == instructorId && !e.IsRandomized)
                .Select(e => new InstructorNonRandomExamsResponseDto
                {
                    ExamId = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    ExamStatus = e.Status,
                    ExamType = e.ExamType,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    TotalMark = e.TotalMark,
                    NumberOfQuestions = e.NumberOfQuestions,
                    DurationInMinutes = e.DurationInMinutes,
                    IsRandomized = e.IsRandomized,
                    PassMarkPercentage = e.PassMarkPercentage,
                    CourseId = e.CourseId,
                    CourseName = e.Course.Name,
                    SectionId = e.SectionId,
                    SectionName = e.Section.Name,
                    
                    // Calculate exam statistics
                    StudentCount = e.ExamResults.Count,
                    PassedCount = e.ExamResults.Count(r => r.Status == ExamResultStatus.Passed),
                    FailedCount = e.ExamResults.Count(r => r.Status == ExamResultStatus.Failed),
                    NotStartedCount = e.ExamResults.Count(r => r.Status == ExamResultStatus.NotStarted),
                    InProgressCount = e.ExamResults.Count(r => r.Status == ExamResultStatus.InProgress)
                }));
        }
    }

}
