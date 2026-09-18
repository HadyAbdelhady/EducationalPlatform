using Application.Common;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payment.Payout
{
    public class GetAdminPayoutSummaryQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetAdminPayoutSummaryQuery, Result<List<PayoutSummaryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<List<PayoutSummaryDto>>> Handle(
            GetAdminPayoutSummaryQuery request, CancellationToken cancellationToken)
        {
            var accounts = await _unitOfWork.Repository<PayoutAccount>()
                .GetAll(cancellationToken)
                .ToListAsync(cancellationToken);

            var result = new List<PayoutSummaryDto>();

            foreach (var acc in accounts)
            {
                var lastBatch = await _unitOfWork.Repository<PayoutBatch>()
                    .Find(b => b.PayeeType == acc.PayeeType && b.PayeeId == acc.PayeeId, cancellationToken)
                    .OrderByDescending(b => b.PeriodYear).ThenByDescending(b => b.PeriodMonth)
                    .FirstOrDefaultAsync(cancellationToken);

                result.Add(new PayoutSummaryDto(
                    acc.PayeeType, acc.PayeeId, acc.Balance,
                    lastBatch?.Status, lastBatch?.PeriodYear, lastBatch?.PeriodMonth, lastBatch?.Net));
            }

            return Result<List<PayoutSummaryDto>>.Success(result);
        }
    }

    public class GetCenterStatementQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetCenterStatementQuery, Result<CenterStatementDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<CenterStatementDto>> Handle(
            GetCenterStatementQuery request, CancellationToken cancellationToken)
        {
            var account = await _unitOfWork.Repository<PayoutAccount>()
                .FirstOrDefaultAsync(
                    a => a.PayeeType == PayeeType.Center && a.PayeeId == request.CenterId,
                    cancellationToken);

            var lastBatch = await _unitOfWork.Repository<PayoutBatch>()
                .Find(b => b.PayeeType == PayeeType.Center && b.PayeeId == request.CenterId, cancellationToken)
                .OrderByDescending(b => b.PeriodYear).ThenByDescending(b => b.PeriodMonth)
                .FirstOrDefaultAsync(cancellationToken);

            var recentLines = await _unitOfWork.Repository<PayoutLedgerLine>()
                .Find(l => l.PayeeType == PayeeType.Center && l.PayeeId == request.CenterId, cancellationToken)
                .OrderByDescending(l => l.CreatedAt)
                .Take(50)
                .ToListAsync(cancellationToken);

            return Result<CenterStatementDto>.Success(
                new CenterStatementDto(account?.Balance ?? 0m, lastBatch, recentLines));
        }
    }

    public class GetInstructorBalanceQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetInstructorBalanceQuery, Result<InstructorBalanceDto?>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<InstructorBalanceDto?>> Handle(
            GetInstructorBalanceQuery request, CancellationToken cancellationToken)
        {
            var account = await _unitOfWork.Repository<PayoutAccount>()
                .FirstOrDefaultAsync(
                    a => a.PayeeType == PayeeType.Instructor && a.PayeeId == request.InstructorId,
                    cancellationToken);

            // Only surface balance if negative (outstanding subscription debt)
            if (account == null || account.Balance >= 0)
                return Result<InstructorBalanceDto?>.Success(null);

            return Result<InstructorBalanceDto?>.Success(new InstructorBalanceDto(account.Balance));
        }
    }
}
