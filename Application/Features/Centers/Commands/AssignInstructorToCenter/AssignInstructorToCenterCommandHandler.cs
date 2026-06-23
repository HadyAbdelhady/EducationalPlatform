using Application.DTOs.Center;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Centers.Commands.AssignInstructorToCenter
{
    public class AssignInstructorToCenterCommandHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<AssignInstructorToCenterCommand, Result<CenterResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<CenterResponse>> Handle(
            AssignInstructorToCenterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var centerRepo = _unitOfWork.GetRepository<ICenterRepository>();

                // Verify center exists via generic repo (Center has Guid Id → implements IEntity)
                var center = await _unitOfWork.Repository<Center>()
                    .GetByIdAsync(request.CenterId, cancellationToken);

                if (center is null || center.IsDeleted)
                    return Result<CenterResponse>.FailureStatusCode("Center not found.", ErrorType.NotFound);

                // Verify instructor exists via ICenterRepository (Instructor uses UserId as PK, not IEntity)
                var instructorExists = await centerRepo
                    .InstructorExistsAsync(request.Request.InstructorId, cancellationToken);

                if (!instructorExists)
                    return Result<CenterResponse>.FailureStatusCode("Instructor not found.", ErrorType.NotFound);

                // Check if instructor is already assigned
                var alreadyAssigned = await centerRepo
                    .IsInstructorInCenterAsync(request.CenterId, request.Request.InstructorId, cancellationToken);

                if (alreadyAssigned)
                    return Result<CenterResponse>.FailureStatusCode(
                        "Instructor is already assigned to this center.", ErrorType.Conflict);

                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                await centerRepo.AddInstructorToCenterAsync(
                    request.CenterId,
                    request.Request.InstructorId,
                    request.Request.EducationYearIds,
                    cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                // Return updated center detail
                var updatedCenter = await centerRepo
                    .GetCenterWithDetailsAsync(request.CenterId, cancellationToken);

                return Result<CenterResponse>.Success(updatedCenter!);
            }
            catch (Exception ex)
            {
                return Result<CenterResponse>.FailureStatusCode(
                    $"Failed to assign instructor to center: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
