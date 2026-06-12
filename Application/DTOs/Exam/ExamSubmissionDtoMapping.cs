using System.Linq.Expressions;
using Domain.Entities;
using Domain.enums;

namespace Application.DTOs.Exam
{
    public static class ExamSubmissionDtoMapping
    {
        public static Expression<Func<StudentExamResult, ExamSubmissionDto>> Project(Domain.Entities.Exam exam)
        {
            return er => new ExamSubmissionDto
            {
                StudentId = er.StudentId,
                StudentName = er.Student != null && er.Student.User != null ? er.Student.User.FullName : string.Empty,
                Status = er.Status,
                ObtainedMarks = er.StudentMark,
                TotalMark = exam.TotalMark,
                TakenAt = er.TakenAt,
                SubmittedAt = er.UpdatedAt ?? er.CreatedAt,
                NumberOfAnswersSubmitted = er.StudentSubmissions != null ? er.StudentSubmissions.Count() : 0,
                TotalQuestions = exam.NumberOfQuestions,
                IsCompleted = er.Status == ExamResultStatus.Passed || er.Status == ExamResultStatus.Failed
            };
        }

        public static ExamSubmissionDto MapFrom(StudentExamResult er, Domain.Entities.Exam exam)
        {
            return new ExamSubmissionDto
            {
                StudentId = er.StudentId,
                StudentName = er.Student?.User?.FullName ?? string.Empty,
                Status = er.Status,
                ObtainedMarks = er.StudentMark,
                TotalMark = exam.TotalMark,
                TakenAt = er.TakenAt,
                SubmittedAt = er.UpdatedAt ?? er.CreatedAt,
                NumberOfAnswersSubmitted = er.StudentSubmissions?.Count ?? 0,
                TotalQuestions = exam.NumberOfQuestions,
                IsCompleted = er.Status == ExamResultStatus.Passed || er.Status == ExamResultStatus.Failed
            };
        }
    }
}
