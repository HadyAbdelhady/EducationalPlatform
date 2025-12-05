namespace Domain.Events
{
    public record ExamDeletedEvent(Guid Id, Guid SectionId, Guid CourseId);
}
