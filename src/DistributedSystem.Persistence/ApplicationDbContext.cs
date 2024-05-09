using DistributedSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DistributedSystem.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder) =>
        builder.ApplyConfigurationsFromAssembly(typeof(AssemblyReference).Assembly);

    public DbSet<Product> Products { get; set; }
}
