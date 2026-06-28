using Domain.enums;
using MediatR;

namespace Application.Features.Exams.Command.ChangeExamStatus
{
    public record ChangeExamStatusCommand(Guid ExamId, ExamStatus Status) : IRequest<bool>;

}
