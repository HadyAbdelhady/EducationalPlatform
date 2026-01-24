using Application.ResultWrapper;
using Application.DTOs.Exam;
using MediatR;

namespace Application.Features.Exams.Query.GetExamById
{
    public class GetExamByIdQuery : IRequest<Result<ExamDetailsQueryModel>>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }
}
