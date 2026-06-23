using Application.DTOs.Center;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Centers.Queries.GetCenterById
{
    public class GetCenterByIdQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetCenterByIdQuery, Result<CenterResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<CenterResponse>> Handle(GetCenterByIdQuery request, CancellationToken cancellationToken)
        {
            var centerRepo = _unitOfWork.GetRepository<ICenterRepository>();
            var center = await centerRepo.GetCenterWithDetailsAsync(request.Id, cancellationToken);

            if (center is null)
                return Result<CenterResponse>.FailureStatusCode("Center not found.", ErrorType.NotFound);

            return Result<CenterResponse>.Success(center);
        }
    }
}
