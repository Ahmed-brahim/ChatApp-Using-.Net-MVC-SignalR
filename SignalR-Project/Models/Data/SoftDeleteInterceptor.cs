using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace SignalR_Project.Models.Data
{
    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            if (eventData.Context == null)
                return result;

            foreach (var entry in eventData.Context.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Deleted && entry.Entity is ISoftDelete softDeleteEntity)
                {
                    entry.State = EntityState.Modified;
                    softDeleteEntity.IsDeleted = true;
                    softDeleteEntity.DeletedAt = DateTime.UtcNow;
                }
            }

            return result;

        }
    }
}
