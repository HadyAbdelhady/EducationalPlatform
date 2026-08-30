using Application.Common;
using Application.Common.Interfaces;
using Application.Features.HomeScreen.DTOs;
using Application.Features.HomeScreen.Interfaces;
using MediatR;

namespace Application.Features.HomeScreen.InstructorAtRisk
{
    public class InstructorAtRiskQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<InstructorAtRiskQuery, Result<AtRiskResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<AtRiskResponse>> Handle(
            InstructorAtRiskQuery request,
            CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<IHomeScreenRepository>();
            var result = await repo.GetInstructorAtRiskAsync(
                request.InstructorId, request.EducationYearId,
                request.Page, request.PageSize, cancellationToken);
            return Result<AtRiskResponse>.Success(result);
        }
    }
}
