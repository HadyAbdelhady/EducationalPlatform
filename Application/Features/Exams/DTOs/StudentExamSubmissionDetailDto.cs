using Application.Common;
using Application.Features.Answers.DTOs;

namespace Application.Features.Exams.DTOs
{
    public class StudentExamQuestionDto
    {
        public Guid Id { get; set; }
        public string QuestionString { get; set; } = string.Empty;
        public string? QuestionImageUrl { get; set; }
        public decimal? QuestionMark { get; set; }
        public Guid SectionId { get; set; }
        public Guid CourseId { get; set; }
        public ICollection<AnswerDto> AllAnswersInExam { get; set; } = [];
        public Guid? ChosenAnswerId { get; set; }
        public bool IsStudentAnswerCorrect { get; set; }
        public decimal ObtainedMark { get; set; }
    }

    public class StudentExamSubmissionDetailDto
    {
        public ExamDetails Exam { get; set; } = new();
        public ExamSubmissionDto Submission { get; set; } = new();
        public PaginatedResult<StudentExamQuestionDto> Questions { get; set; } = new();
    }
}
