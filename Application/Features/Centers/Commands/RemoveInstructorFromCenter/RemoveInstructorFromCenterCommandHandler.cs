using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Centers.Commands.RemoveInstructorFromCenter
{
    public class RemoveInstructorFromCenterCommandHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<RemoveInstructorFromCenterCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<bool>> Handle(
            RemoveInstructorFromCenterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var centerRepo = _unitOfWork.GetRepository<ICenterRepository>();

                var isAssigned = await centerRepo
                    .IsInstructorInCenterAsync(request.CenterId, request.InstructorId, cancellationToken);

                if (!isAssigned)
                    return Result<bool>.FailureStatusCode(
                        "Instructor is not a member of this center.", ErrorType.NotFound);

                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                // Delegate junction-table writes to the repository (Infrastructure concern)
                await centerRepo.RemoveInstructorFromCenterAsync(
                    request.CenterId,
                    request.InstructorId,
                    cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureStatusCode(
                    $"Failed to remove instructor from center: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
