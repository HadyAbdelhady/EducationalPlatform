using Application.DTOs.Center;
using Application.Interfaces;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Centers.Queries.GetAllCenters
{
    public class GetAllCentersQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetAllCentersQuery, Result<IEnumerable<CenterSummaryResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<IEnumerable<CenterSummaryResponse>>> Handle(
            GetAllCentersQuery request, CancellationToken cancellationToken)
        {
            var centerRepo = _unitOfWork.GetRepository<ICenterRepository>();
            var centers = await centerRepo.GetAllCentersAsync(cancellationToken);

            return Result<IEnumerable<CenterSummaryResponse>>.Success(centers);
        }
    }
}
