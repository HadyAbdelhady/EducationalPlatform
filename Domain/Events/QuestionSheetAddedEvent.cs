using Domain.enums;
using MediatR;

namespace Domain.Events
{
    public record QuestionSheetAddedEvent(Guid EntityId, EntityType EntityType) : INotification
    {
    }
}
