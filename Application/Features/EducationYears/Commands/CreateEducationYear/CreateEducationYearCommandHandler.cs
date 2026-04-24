using Application.DTOs.EducationYear;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.EducationYears.Commands.CreateEducationYear
{
    public class CreateEducationYearCommandHandler(IUnitOfWork unitOfWork) 
        : IRequestHandler<CreateEducationYearCommand, Result<EducationYearResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<EducationYearResponse>> Handle(CreateEducationYearCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<IEducationYearRepository>();
            
            // Check if education year with same name already exists
            var existingYears = await repo.GetActiveEducationYearsAsync(cancellationToken);
            if (existingYears.Any(ey => ey.EducationYearName.Equals(request.EducationYear.EducationYearName, StringComparison.OrdinalIgnoreCase)))
            {
                return Result<EducationYearResponse>.Failure(
                    "An education year with this name already exists.",
                    ResultType.Conflict
                );
            }

            var educationYear = new EducationYear
            {
                Id = Guid.NewGuid(),
                EducationYearName = request.EducationYear.EducationYearName,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _unitOfWork.EducationYears.AddAsync(educationYear, cancellationToken);
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
