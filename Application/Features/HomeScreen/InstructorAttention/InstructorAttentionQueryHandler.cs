using Application.Common;
using Application.Common.Interfaces;
using Application.Features.HomeScreen.DTOs;
using Application.Features.HomeScreen.Interfaces;
using MediatR;

namespace Application.Features.HomeScreen.InstructorAttention
{
    public class InstructorAttentionQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<InstructorAttentionQuery, Result<AttentionResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<AttentionResponse>> Handle(
            InstructorAttentionQuery request,
            CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<IHomeScreenRepository>();
            var result = await repo.GetInstructorAttentionAsync(
                request.InstructorId, request.EducationYearId, cancellationToken);
            return Result<AttentionResponse>.Success(result);
        }
    }
}
