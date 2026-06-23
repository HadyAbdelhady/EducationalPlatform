using Application.DTOs.Center;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Centers.Queries.GetAllCenters
{
    public class GetAllCentersQuery : IRequest<Result<IEnumerable<CenterSummaryResponse>>>
    {
    }
}
