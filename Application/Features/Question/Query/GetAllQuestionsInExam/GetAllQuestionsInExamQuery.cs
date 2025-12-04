using Application.DTOs.Question;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Question.Query.GetAllQuestionsInExam
{
    public class GetAllQuestionsInExamQuery : IRequest<Result<PaginatedResult<GetAllQuestionsInExamResponse>>>
    {
        public Guid ExamId { get; set; }
    }
}
