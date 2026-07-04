using Application.Features.EducationYears.DTOs;
using Application.Common.Interfaces;
using Application.Features.EducationYears.Interfaces;
using Application.Common;
using MediatR;

namespace Application.Features.EducationYears.Queries.GetEducationYears
{
    public class GetEducationYearsQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetEducationYearsQuery, Result<List<EducationYearDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<List<EducationYearDto>>> Handle(GetEducationYearsQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<IEducationYearRepository>();
            var educationYears = await repo.GetActiveEducationYearsForInstructorAsync(request.InstructorId, request.ApplicationName);
            return Result<List<EducationYearDto>>.Success(educationYears);
        }
    }
}
