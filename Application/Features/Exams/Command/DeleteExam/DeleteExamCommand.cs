using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Exams.Command.DeleteExam
{
    public class DeleteExamCommand : IRequest<Result<string>>
    {
        public Guid ExamId { get; set; }
        public Guid SectionId { get; set; }
        public Guid CourseId { get; set; }

    }
}
