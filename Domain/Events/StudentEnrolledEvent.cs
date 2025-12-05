namespace Domain.Events
{
    public record StudentEnrolledEvent(Guid StudentId, Guid EnrollmentEntityId);
}
