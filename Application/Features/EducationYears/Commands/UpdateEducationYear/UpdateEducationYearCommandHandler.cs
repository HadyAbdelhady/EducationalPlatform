using Application.DTOs.EducationYear;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.EducationYears.Commands.UpdateEducationYear
{
    public class UpdateEducationYearCommandHandler(IUnitOfWork unitOfWork) 
        : IRequestHandler<UpdateEducationYearCommand, Result<EducationYearResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<EducationYearResponse>> Handle(UpdateEducationYearCommand request, CancellationToken cancellationToken)
        {
            var educationYear = await _unitOfWork.EducationYears
                .FirstOrDefaultAsync(ey => ey.Id == request.Id && !ey.IsDeleted, cancellationToken);

            if (educationYear == null)
            {
                return Result<EducationYearResponse>.Failure(
                    "Education year not found.",
                    ResultType.NotFound
                );
            }

            var repo = _unitOfWork.GetRepository<IEducationYearRepository>();
            
            // Check if another education year with the same name already exists
            var existingYears = await repo.GetActiveEducationYearsAsync(cancellationToken);
            if (existingYears.Any(ey => ey.Id != request.Id && 
                ey.EducationYearName.Equals(request.EducationYear.EducationYearName, StringComparison.OrdinalIgnoreCase)))
            {
                return Result<EducationYearResponse>.Failure(
                    "Another education year with this name already exists.",
                    ResultType.Conflict
                );
            }

            educationYear.EducationYearName = request.EducationYear.EducationYearName;
            educationYear.UpdatedAt = DateTimeOffset.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new EducationYearResponse
            {
                Id = educationYear.Id,
                EducationYearName = educationYear.EducationYearName,
                CreatedAt = educationYear.CreatedAt,
                UpdatedAt = educationYear.UpdatedAt
            };

            return Result<EducationYearResponse>.Success(response);
        }
    }
}
