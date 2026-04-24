using Application.DTOs.EducationYear;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.EducationYears.Queries.GetEducationYearById
{
    public class GetEducationYearByIdQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetEducationYearByIdQuery, Result<EducationYearResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<EducationYearResponse>> Handle(GetEducationYearByIdQuery request, CancellationToken cancellationToken)
        {
            var educationYear = await _unitOfWork.GetRepository<IEducationYearRepository>()
                .GetEducationYearByIdAsync(request.Id, cancellationToken);

            if (educationYear == null)
            {
                return Result<EducationYearResponse>.FailureStatusCode(
                    "Education year not found.",
                    ErrorType.NotFound
                );
            }

            return Result<EducationYearResponse>.Success(new EducationYearResponse
            {
                Id = educationYear.Id,
                EducationYearName = educationYear.EducationYearName,
            });
        }
    }
}
