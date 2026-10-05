using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EnterprisePlatform.Infrastructure.Persistence.Interceptors
{
    public class ConcurrencyInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
        {
            UpdateVersions(eventData.Context);

            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
        {
            UpdateVersions(eventData.Context);

            return base.SavingChangesAsync(
                eventData,
                result,
                cancellationToken);
        }

        private static void UpdateVersions(DbContext? context)
        {
            if (context is null)
            {
                return;
            }

            var entries = context.ChangeTracker
                .Entries()
                .Where(x =>
                    x.State == EntityState.Modified &&
                    x.Metadata.FindProperty("Version") is not null);

            foreach (var entry in entries)
            {
                var currentVersion =
                    (long)entry.Property("Version").CurrentValue!;

                entry.Property("Version").CurrentValue =
                    currentVersion + 1;
            }
        }

    }
}