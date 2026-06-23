using Application.DTOs.Center;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Centers.Commands.UpdateCenter
{
    public class UpdateCenterCommandHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<UpdateCenterCommand, Result<CenterResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<CenterResponse>> Handle(UpdateCenterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var center = await _unitOfWork.Repository<Center>()
                    .GetByIdAsync(request.Id, cancellationToken);

                if (center is null || center.IsDeleted)
                    return Result<CenterResponse>.FailureStatusCode(
                        "Center not found.", ErrorType.NotFound);

                // Apply partial updates — only update fields that were provided
                if (!string.IsNullOrWhiteSpace(request.Request.Name))
                    center.Name = request.Request.Name;

                if (request.Request.Description is not null)
                    center.Description = request.Request.Description;

                if (request.Request.LogoUrl is not null)
                    center.LogoUrl = request.Request.LogoUrl;

                if (request.Request.LocationMaps is not null)
                    center.LocationMaps = request.Request.LocationMaps;

                center.UpdatedAt = EgyptTime.UtcNow;

                _unitOfWork.Repository<Center>().Update(center);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<CenterResponse>.Success(new CenterResponse
                {
                    Id = center.Id,
                    Name = center.Name,
                    Description = center.Description,
                    LogoUrl = center.LogoUrl,
                    LocationMaps = center.LocationMaps,
                    CreatedAt = center.CreatedAt
                });
            }
            catch (Exception ex)
            {
                return Result<CenterResponse>.FailureStatusCode(
                    $"Failed to update center: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
