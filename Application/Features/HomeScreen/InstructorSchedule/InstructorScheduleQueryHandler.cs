using Application.Common;
using Application.Common.Interfaces;
using Application.Features.HomeScreen.DTOs;
using Application.Features.HomeScreen.Interfaces;
using Domain.enums;
using MediatR;

namespace Application.Features.HomeScreen.InstructorSchedule
{
    public class InstructorScheduleQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<InstructorScheduleQuery, Result<InstructorScheduleResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<InstructorScheduleResponse>> Handle(
            InstructorScheduleQuery request,
            CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<IHomeScreenRepository>();
            var result = await repo.GetInstructorScheduleAsync(
                request.InstructorId, request.EducationYearId, request.Days, cancellationToken);
            return Result<InstructorScheduleResponse>.Success(result);
        }
    }
}
