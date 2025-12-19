using Application.DTOs.Questions;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Questions.Query.GetAllQuestionsInExam
{
    public class GetAllQuestionsInExamQuery : IRequest<Result<PaginatedResult<GetAllQuestionsInExamResponse>>>
    {
        public Guid ExamId { get; set; }
    }
}
