using Application.Features.Centers.DTOs;
using Application.Common;
using MediatR;

namespace Application.Features.Centers.Queries.GetAllCenters
{
    public class GetAllCentersQuery : IRequest<Result<IEnumerable<CenterSummaryResponse>>>
    {
    }
}
