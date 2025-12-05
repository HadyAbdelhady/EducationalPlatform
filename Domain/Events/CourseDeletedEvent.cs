namespace Domain.Events
{
    public record CourseDeletedEvent(Guid CourseId, Guid InstructorId, Guid StudentId);
}
