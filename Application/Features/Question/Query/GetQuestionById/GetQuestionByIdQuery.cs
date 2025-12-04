using Application.DTOs.Question;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Question.Query.GetQuestionById
{
    public class GetQuestionByIdQuery : IRequest<Result<QuestionDetailsResponse>>
    {
        public Guid QuestionId { get; set; }
    }
}
