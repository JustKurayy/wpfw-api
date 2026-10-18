using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WpfwApi.Data;

namespace WpfwApi.Tests.Infrastructure;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string? _connectionString;

    public ApiFactory()
    {
    }

    public ApiFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        if (_connectionString is null)
        {
            return;
        }

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<PortfolioDbContext>>();
            services.RemoveAll<PortfolioDbContext>();
            services.AddDbContext<PortfolioDbContext>(options => options.UseSqlServer(_connectionString));
        });
    }

    public async Task MigrateDatabaseAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
        await context.Database.MigrateAsync();
    }

    public async Task DeleteDatabaseAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
        await context.Database.EnsureDeletedAsync();
    }
}
