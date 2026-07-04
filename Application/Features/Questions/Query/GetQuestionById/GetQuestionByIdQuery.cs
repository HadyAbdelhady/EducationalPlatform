using Application.Features.Questions.DTOs;
using Application.Common;
using MediatR;

namespace Application.Features.Questions.Query.GetQuestionById
{
    public class GetQuestionByIdQuery : IRequest<Result<QuestionDetailsResponse>>
    {
        public Guid QuestionId { get; set; }
    }
}
