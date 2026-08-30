using Application.Common;
using Application.Common.Interfaces;
using Application.Features.HomeScreen.DTOs;
using Application.Features.HomeScreen.Interfaces;
using MediatR;

namespace Application.Features.HomeScreen.InstructorPayments
{
    public class InstructorPaymentsQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<InstructorPaymentsQuery, Result<InstructorPaymentsResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<InstructorPaymentsResponse>> Handle(
            InstructorPaymentsQuery request,
            CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<IHomeScreenRepository>();
            var result = await repo.GetInstructorPaymentsAsync(
                request.InstructorId, request.EducationYearId, request.Days, cancellationToken);
            return Result<InstructorPaymentsResponse>.Success(result);
        }
    }
}
