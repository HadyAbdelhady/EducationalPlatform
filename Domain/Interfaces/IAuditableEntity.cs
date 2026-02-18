namespace Domain.Interfaces
{
    /// <summary>
    /// Interface for entities with audit tracking properties.
    /// </summary>
    public interface IAuditableEntity : IEntity
    {
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset? UpdatedAt { get; set; }
    }
}
