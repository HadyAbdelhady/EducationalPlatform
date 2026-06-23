using Application.DTOs.Center;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Centers.Commands.CreateCenter
{
    public class CreateCenterCommandHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<CreateCenterCommand, Result<CenterResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<CenterResponse>> Handle(CreateCenterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check for duplicate name
                var nameExists = await _unitOfWork.Repository<Center>()
                    .AnyAsync(c => c.Name == request.Request.Name && !c.IsDeleted, cancellationToken);

                if (nameExists)
                    return Result<CenterResponse>.FailureStatusCode(
                        $"A center with the name '{request.Request.Name}' already exists.",
                        ErrorType.Conflict);

                var center = new Center
                {
                    Id = Guid.NewGuid(),
                    Name = request.Request.Name,
                    Description = request.Request.Description,
                    LogoUrl = request.Request.LogoUrl,
                    LocationMaps = request.Request.LocationMaps,
                    CreatedAt = EgyptTime.UtcNow
                };

                await _unitOfWork.Repository<Center>().AddAsync(center, cancellationToken);
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
                    $"Failed to create center: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
