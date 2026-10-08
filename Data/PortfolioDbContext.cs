using Microsoft.EntityFrameworkCore;
using WpfwApi.Models;

namespace WpfwApi.Data;

public sealed class PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PortfolioDbContext).Assembly);
    }
}
