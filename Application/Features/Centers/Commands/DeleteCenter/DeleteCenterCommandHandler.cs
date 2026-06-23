using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Centers.Commands.DeleteCenter
{
    public class DeleteCenterCommandHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<DeleteCenterCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<bool>> Handle(DeleteCenterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var center = await _unitOfWork.Repository<Center>()
                    .GetByIdAsync(request.Id, cancellationToken);

                if (center is null || center.IsDeleted)
                    return Result<bool>.FailureStatusCode("Center not found.", ErrorType.NotFound);

                // Soft delete — the SoftDeleteInterceptor handles IsDeleted = true automatically,
                // but we can also set it explicitly for clarity
                center.IsDeleted = true;
                center.UpdatedAt = EgyptTime.UtcNow;

                _unitOfWork.Repository<Center>().Update(center);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureStatusCode(
                    $"Failed to delete center: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
