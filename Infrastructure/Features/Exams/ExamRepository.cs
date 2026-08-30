using Infrastructure.Common;
using Application.Features.Exams.Command.UpdateExam;
using Microsoft.EntityFrameworkCore;
using Application.Features.Questions.DTOs;
using Application.Features.Answers.DTOs;
using Application.Common.Interfaces;
using Application.Features.Exams.Interfaces;
using Application.Features.Exams.DTOs;
using Infrastructure.Common.Data;
using Domain;
using Domain.Entities;
using Domain.enums;

namespace Infrastructure.Features.Exams
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



        public async Task<ExamDetailsQueryModel?> GetExamByIdWithQuestionsAndAnswersAsync(Guid ExamId, CancellationToken cancellationToken = default)
        {
            if (ExamId == Guid.Empty)
            {
                throw new ArgumentException("ExamId cannot be empty.");
            }
            return await _context.Exams
                                 .Where(e => e.Id == ExamId)
                                 .Select(e => new ExamDetailsQueryModel
                                 {
                                     ExamId = e.Id,
                                     Title = e.Name,
                                     Description = e.Description,
                                     ScheduledDate = e.StartTime,
                                     DurationInMinutes = e.DurationInMinutes,
                                     NumberOfQuestions = e.NumberOfQuestions,
                                     TotalMark = e.TotalMark,
                                     ExamType = e.ExamType,
                                     EndDate = e.EndTime,
                                     AllQuestionsInExam = e.ExamQuestions
                                         .Select(eq => new QuestionsInExamWithAnswersResponse
                                         {
                                             Id = eq.Question.Id,
                                             QuestionMark = eq.QuestionMark,
                                             CourseId = eq.Question.CourseId,
                                             QuestionImageUrl = eq.Question.QuestionImageUrl,
                                             QuestionString = eq.Question.QuestionString ?? string.Empty,
                                             SectionId = eq.Question.SectionId ?? Guid.Empty,
                                             AllAnswersInExam = eq.Question.Answers
                                                 .Where(a => !a.IsDeleted)
                                                 .Select(a => new AnswerDto
                                                 {
                                                     Id = a.Id,
                                                     AnswerString = a.AnswerText,
                                                     IsCorrect = a.IsCorrect,
                                                     QuestionId = a.QuestionId ?? Guid.Empty
                                                 }).ToList()
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

        public async Task<CoursesSectionsHashMap> GetInstructorCoursesSectionsHashMapAsync(Guid instructorId, CancellationToken cancellationToken)
        {
            var courses = await _context.Courses
                .AsNoTracking()
                .Where(c => c.InstructorCourses.Any(ic => ic.InstructorId == instructorId))
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    Sections = c.Sections.Select(s => new { s.Id, s.Name })
                })
                .ToListAsync(cancellationToken);

            var hashMap = new CoursesSectionsHashMap();

            foreach (var course in courses)
            {
                hashMap.Courses[course.Id] = new CourseSectionInfo
                {
                    Id = course.Id,
                    Name = course.Name,
                    Sections = course.Sections.ToDictionary(
                        s => s.Id,
                        s => new SectionInfo { Id = s.Id, Name = s.Name })
                };
            }

            return hashMap;
        }

        public IQueryable<InstructorExamsResponseDto> GetInstructorNonRandomExamsQuery(Guid instructorId)
        {
            return _context.Exams
                .AsNoTracking()
                .Where(e => e.InstructorId == instructorId && !e.IsDeleted)
                .Select(e => new InstructorExamsResponseDto
                {
                    ExamId = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    ExamStatus = e.Status == ExamStatus.Draft
                        ? ExamStatus.Draft
                        : (e.Status == ExamStatus.Finished || (e.EndTime != null && e.EndTime <= EgyptTime.UtcNow)
                            ? ExamStatus.Finished
                            : (e.StartTime != null && e.StartTime <= EgyptTime.UtcNow
                                ? ExamStatus.Started
                                : ExamStatus.Scheduled)),
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
                    CourseName = e.Course != null ? e.Course.Name : string.Empty,
                    SectionId = e.SectionId,
                    SectionName = e.Section != null ? e.Section.Name : null,
                    StudentCount =
                        _context.StudentCourses.Count(sc => sc.CourseId == e.CourseId)
                        + _context.StudentSections.Count(ss =>
                            e.SectionId != null
                            && ss.SectionId == e.SectionId
                            && !_context.StudentCourses.Any(sc =>
                                sc.CourseId == e.CourseId && sc.StudentId == ss.StudentId)),
                    PassedCount = e.ExamResults.Count(r => r.Status == ExamResultStatus.Passed),
                    FailedCount = e.ExamResults.Count(r => r.Status == ExamResultStatus.Failed),
                    NotStartedCount =
                        _context.StudentCourses.Count(sc => sc.CourseId == e.CourseId)
                        + _context.StudentSections.Count(ss =>
                            e.SectionId != null
                            && ss.SectionId == e.SectionId
                            && !_context.StudentCourses.Any(sc =>
                                sc.CourseId == e.CourseId && sc.StudentId == ss.StudentId))
                        - e.ExamResults.Count(r => r.Status != ExamResultStatus.NotStarted),
                    InProgressCount = e.ExamResults.Count(r => r.Status == ExamResultStatus.InProgress)
                });
        }
    }

}
