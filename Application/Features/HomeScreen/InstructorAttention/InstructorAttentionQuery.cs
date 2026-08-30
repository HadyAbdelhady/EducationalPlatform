using Application.Common;
using Application.Features.HomeScreen.DTOs;
using MediatR;

namespace Application.Features.HomeScreen.InstructorAttention
{
    public class InstructorAttentionQuery : IRequest<Result<AttentionResponse>>
    {
        public Guid InstructorId { get; set; }
        public Guid EducationYearId { get; set; }
    }
}
