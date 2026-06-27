using System.Linq.Expressions;
using Domain.Entities;
using Domain.enums;

namespace Application.DTOs.Exam
{
    public record ExamSubmissionProjectionContext(
        Guid ExamId,
        string ExamName,
        decimal TotalMark,
        int NumberOfQuestions,
        int PassMarkPercentage,
        string CourseName,
        int? Duration,
        string SectionName)
    {
        public static ExamSubmissionProjectionContext FromExam(Domain.Entities.Exam exam) => new(
            exam.Id,
            exam.Name,
            exam.TotalMark,
            exam.NumberOfQuestions,
            exam.PassMarkPercentage,
            exam.Course?.Name ?? string.Empty,
            exam.DurationInMinutes,
            exam.Section?.Name ?? string.Empty);
    }

    public static class ExamSubmissionDtoMapping
    {
        public static ExamDetails ToExamDetails(Domain.Entities.Exam exam) => new()
        {
            ExamId = exam.Id,
            ExamName = exam.Name,
            TotalMark = exam.TotalMark,
            NumberOfQuestions = exam.NumberOfQuestions,
            CourseName = exam.Course?.Name ?? string.Empty,
            SectionName = exam.Section?.Name ?? string.Empty,
            EducatunalYearName = exam.Course?.EducationYear?.EducationYearName ?? string.Empty,
            IsRandomized = exam.IsRandomized,
            ExamType = exam.ExamType,
            StartDate = exam.StartTime,
            EndDate = exam.EndTime,
        };

        public static Expression<Func<StudentExamResult, ExamSubmissionDto>> Project(ExamSubmissionProjectionContext exam)
        {
            var totalMark = exam.TotalMark;
            var numberOfQuestions = exam.NumberOfQuestions;
            var passMarkPercentage = exam.PassMarkPercentage;

            return er => new ExamSubmissionDto
            {
                StudentId = er.StudentId,
                StudentName = er.Student != null && er.Student.User != null ? er.Student.User.FullName : string.Empty,
                Status = er.Status,
                ObtainedMarks = er.StudentMark,
                TotalMark = totalMark,
                PassMarkPercentage = passMarkPercentage,
                TakenAt = er.TakenAt,
                SubmittedAt = er.UpdatedAt ?? er.CreatedAt,
                NumberOfAnswersSubmitted = er.StudentSubmissions.Count,
                TotalQuestions = numberOfQuestions,
                IsCompleted = er.Status == ExamResultStatus.Passed || er.Status == ExamResultStatus.Failed
            };
        }

        public static ExamSubmissionDto MapFrom(StudentExamResult er, ExamSubmissionProjectionContext exam)
        {
            return new ExamSubmissionDto
            {
                StudentId = er.StudentId,
                StudentName = er.Student?.User?.FullName ?? string.Empty,
                Status = er.Status,
                ObtainedMarks = er.StudentMark,
                TotalMark = exam.TotalMark,
                PassMarkPercentage = exam.PassMarkPercentage,
                TakenAt = er.TakenAt,
                SubmittedAt = er.UpdatedAt ?? er.CreatedAt,
                NumberOfAnswersSubmitted = er.StudentSubmissions?.Count ?? 0,
                TotalQuestions = exam.NumberOfQuestions,
                IsCompleted = er.Status == ExamResultStatus.Passed || er.Status == ExamResultStatus.Failed
            };
        }
    }
}
