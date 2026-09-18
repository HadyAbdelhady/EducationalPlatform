using Application.Common;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Payment.Payout
{
    // ── Admin: all payee balances + latest batch ──────────────────────────────
    public record GetAdminPayoutSummaryQuery : IRequest<Result<List<PayoutSummaryDto>>>;

    // ── CenterAdmin: center statement ─────────────────────────────────────────
    public record GetCenterStatementQuery(Guid CenterId) : IRequest<Result<CenterStatementDto>>;

    // ── Instructor: own balance (only if negative) ────────────────────────────
    public record GetInstructorBalanceQuery(Guid InstructorId) : IRequest<Result<InstructorBalanceDto?>>;

    // ── DTOs ──────────────────────────────────────────────────────────────────
    public record PayoutSummaryDto(
        PayeeType PayeeType, Guid PayeeId, decimal Balance, PayoutBatchStatus? LastBatchStatus,
        int? LastPeriodYear, int? LastPeriodMonth, decimal? LastNet);

    public record CenterStatementDto(
        decimal Balance, PayoutBatch? LastBatch, List<PayoutLedgerLine> RecentLines);

    public record InstructorBalanceDto(decimal Balance);
}
