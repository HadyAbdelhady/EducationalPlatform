namespace Domain.Interfaces
{
    /// <summary>
    /// Interface for entities that support soft delete.
    /// </summary>
    public interface ISoftDeletableEntity : IAuditableEntity
    {
        bool IsDeleted { get; set; }
    }
}
