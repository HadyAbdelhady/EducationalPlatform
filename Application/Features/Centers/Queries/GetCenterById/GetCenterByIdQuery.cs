using Application.DTOs.Center;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Centers.Queries.GetCenterById
{
    public class GetCenterByIdQuery : IRequest<Result<CenterResponse>>
    {
        public Guid Id { get; set; }
    }
}
