using Application.DTOs.Exam;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Exams.Query.GetExamCalendarDays
{
    public class GetExamCalendarDaysQuery : IRequest<Result<List<ExamCalendarItemDto>>>
    {
        public Guid? CourseId { get; set; }
        public Guid? SectionId { get; set; }
        public Guid EducationYearId { get; set; }
        public Guid InstructorId { get; set; }
    }
}

