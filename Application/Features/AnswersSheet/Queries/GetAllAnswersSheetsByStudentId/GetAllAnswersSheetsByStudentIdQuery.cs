using Application.DTOs.Sheets;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.AnswersSheets.Queries.GetAllAnswersSheetsByStudentId
{
    public class GetAllAnswersSheetsByStudentIdQuery : IRequest<Result<PaginatedResult<AllAnswersSheetsByStudentResponse>>>
    {
        public Guid StudentId { get; set; }
    }
}

