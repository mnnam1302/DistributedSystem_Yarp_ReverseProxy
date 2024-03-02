using DistributedSystem.Domain.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DistributedSystem.Persistence.Interceptors
{
    public sealed class UpdateAuditableEntitiesInterceptor
        : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            DbContext? dbContext = eventData.Context;

            if (dbContext is null)
            {
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            IEnumerable<EntityEntry<IAuditableEntity>> entites =
                dbContext.ChangeTracker
                .Entries<IAuditableEntity>();

            // Sau này, chúng ta tổng quát hóa lên một chút
            // Kiểu thay đổi như này, tự động lấy theo kiểu User nào tác động
            // Lấy theo kiểu Id của User login trong HttpContext
            // Có 4 trường: CreatedBy, CreatedOnUtc, ModifiedBy, ModifiedOnUtc - mình mới triển khai 2 trường

            foreach (EntityEntry<IAuditableEntity> entityEntry in entites)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    entityEntry.Property(x => x.CreatedOnUtc).CurrentValue = DateTime.UtcNow;
                }

                if (entityEntry.State == EntityState.Modified)
                {
                    entityEntry.Property(x => x.ModifiedOnUtc).CurrentValue = DateTime.UtcNow;
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}