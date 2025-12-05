namespace Domain.Events
{
    public record SectionDeletedEvent(Guid SectionId, Guid CourseId);
}
