using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Common.Persistence.Interceptors
{
    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            ApplySoftDelete(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            ApplySoftDelete(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void ApplySoftDelete(DbContext? context)
        {
            if (context is null) return;

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.State != EntityState.Deleted)
                    continue;

                var isDeletedProp = entry.Metadata.FindProperty("IsDeleted");
                if (isDeletedProp == null || isDeletedProp.ClrType != typeof(bool))
                    continue;

                entry.State = EntityState.Modified;
                entry.Property("IsDeleted").CurrentValue = true;
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
    }
}
