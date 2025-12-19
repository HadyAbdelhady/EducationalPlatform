using Application.DTOs.Questions;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Questions.Query.GetQuestionById
{
    public class GetQuestionByIdQuery : IRequest<Result<QuestionDetailsResponse>>
    {
        public Guid QuestionId { get; set; }
    }
}
