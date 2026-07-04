using Application.Features.Centers.DTOs;
using Application.Common;
using MediatR;

namespace Application.Features.Centers.Queries.GetCenterById
{
    public class GetCenterByIdQuery : IRequest<Result<CenterResponse>>
    {
        public Guid Id { get; set; }
    }
}
