using Application.DTOs.EducationYear;
using Application.ResultWrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.EducationYears.Queries.GetEducationYearById
{
    public class GetEducationYearByIdQueryHandler(IUnitOfWork unitOfWork) 
        : IRequestHandler<GetEducationYearByIdQuery, Result<EducationYearResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<EducationYearResponse>> Handle(GetEducationYearByIdQuery request, CancellationToken cancellationToken)
        {
            var educationYear = await _unitOfWork.EducationYears
                .AsNoTracking()
                .Where(ey => ey.Id == request.Id && !ey.IsDeleted)
                .Select(ey => new EducationYearResponse
                {
                    Id = ey.Id,
                    EducationYearName = ey.EducationYearName,
                    CreatedAt = ey.CreatedAt,
                    UpdatedAt = ey.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (educationYear == null)
            {
                return Result<EducationYearResponse>.Failure(
                    "Education year not found.",
                    ResultType.NotFound
                );
            }

            return Result<EducationYearResponse>.Success(educationYear);
        }
    }
}
