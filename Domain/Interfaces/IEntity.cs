namespace Domain.Interfaces
{
    /// <summary>
    /// Base interface for entities with a GUID identifier.
    /// </summary>
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}
