using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors
{
    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData.Context is null) return result;

            foreach (var entry in eventData.Context.ChangeTracker.Entries())
            {
                if (entry is not { State: EntityState.Deleted, Entity: ISoftDeletableEntity softDeleteEntity })
                    continue;

                entry.State = EntityState.Modified;
                softDeleteEntity.IsDeleted = true;
                entry.Property("IsDeleted").IsModified = true;

                
                // Mark all other properties as not modified to prevent overwriting
                foreach (var property in entry.Properties)
                {
                    if (property.Metadata.Name != "IsDeleted")
                    {
                        property.IsModified = false;
                    }
                }
            }

            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is not null)
            {
                foreach (var entry in eventData.Context.ChangeTracker.Entries())
                {
                    if (entry is not { State: EntityState.Deleted, Entity: ISoftDeletableEntity softDeleteEntity })
                        continue;

                    entry.State = EntityState.Modified;
                    softDeleteEntity.IsDeleted = true;
                    entry.Property("IsDeleted").IsModified = true;

                    // Mark all other properties as not modified to prevent overwriting
                    foreach (var property in entry.Properties)
                    {
                        if (property.Metadata.Name != "IsDeleted")
                        {
                            property.IsModified = false;
                        }
                    }
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
