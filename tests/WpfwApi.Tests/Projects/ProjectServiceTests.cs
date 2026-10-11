using System.ComponentModel.DataAnnotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WpfwApi.Data;
using WpfwApi.Dtos.Projects;
using WpfwApi.Models;
using WpfwApi.Services.Projects;

namespace WpfwApi.Tests.Projects;

public sealed class ProjectServiceTests
{
    [Fact]
    public void CreateRequest_RejectsWhitespaceTitle_AndDoesNotExposeId()
    {
        var request = new CreateProjectRequest
        {
            Titel = " ",
            Beschrijving = "Een geldige beschrijving",
            Categorie = "Web",
            GitHubUrl = "https://github.com/voorbeeld/project",
            Datum = new DateTime(2026, 10, 26)
        };

        var results = Validate(request);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(request.Titel)));
        Assert.Null(typeof(CreateProjectRequest).GetProperty("Id"));
    }

    [Fact]
    public async Task CreateAsync_PersistsProjectAndReturnsResponse()
    {
        await using var database = await ProjectTestDatabase.CreateAsync();
        var service = new ProjectService(database.Context);
        var request = ValidCreateRequest();

        var response = await service.CreateAsync(request, CancellationToken.None);

        Assert.True(response.Id > 0);
        Assert.Equal(request.Titel, response.Titel);
        Assert.Equal(1, await database.Context.Projects.CountAsync());
    }

    [Fact]
    public async Task GetMethods_ReturnPersistedDtos()
    {
        await using var database = await ProjectTestDatabase.CreateAsync();
        database.Context.Projects.Add(new Project
        {
            Titel = "Portfolio API",
            Beschrijving = "Backend voor mijn portfolio",
            Categorie = "Backend",
            GitHubUrl = "https://github.com/voorbeeld/portfolio-api",
            Datum = new DateTime(2026, 10, 26)
        });
        await database.Context.SaveChangesAsync();
        var service = new ProjectService(database.Context);

        var all = await service.GetAllAsync(CancellationToken.None);
        var single = await service.GetByIdAsync(all[0].Id, CancellationToken.None);

        Assert.Single(all);
        Assert.NotNull(single);
        Assert.Equal("Portfolio API", single.Titel);
    }

    [Fact]
    public async Task UpdateAsync_ChangesExistingProject_AndReturnsNullForMissingId()
    {
        await using var database = await ProjectTestDatabase.CreateAsync();
        var service = new ProjectService(database.Context);
        var created = await service.CreateAsync(ValidCreateRequest(), CancellationToken.None);
        var update = new UpdateProjectRequest
        {
            Titel = "Gewijzigde API",
            Beschrijving = "De bijgewerkte beschrijving",
            Categorie = "C#",
            GitHubUrl = "https://github.com/voorbeeld/gewijzigd",
            Datum = new DateTime(2026, 10, 27)
        };

        var changed = await service.UpdateAsync(created.Id, update, CancellationToken.None);
        var missing = await service.UpdateAsync(99_999, update, CancellationToken.None);

        Assert.Equal("Gewijzigde API", changed!.Titel);
        Assert.Null(missing);
    }

    [Fact]
    public async Task DeleteAsync_RemovesExistingProject_AndReturnsFalseForMissingId()
    {
        await using var database = await ProjectTestDatabase.CreateAsync();
        var service = new ProjectService(database.Context);
        var created = await service.CreateAsync(ValidCreateRequest(), CancellationToken.None);

        var deleted = await service.DeleteAsync(created.Id, CancellationToken.None);
        var missing = await service.DeleteAsync(created.Id, CancellationToken.None);

        Assert.True(deleted);
        Assert.False(missing);
        Assert.Empty(await database.Context.Projects.ToListAsync());
    }

    private static CreateProjectRequest ValidCreateRequest() => new()
    {
        Titel = "Portfolio API",
        Beschrijving = "Backend voor mijn portfolio",
        Categorie = "Backend",
        GitHubUrl = "https://github.com/voorbeeld/portfolio-api",
        Datum = new DateTime(2026, 10, 26)
    };

    private static IReadOnlyList<ValidationResult> Validate(object value)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(value, new ValidationContext(value), results, true);
        return results;
    }

    private sealed class ProjectTestDatabase : IAsyncDisposable
    {
        private readonly SqliteConnection _connection;

        private ProjectTestDatabase(SqliteConnection connection, PortfolioDbContext context)
        {
            _connection = connection;
            Context = context;
        }

        public PortfolioDbContext Context { get; }

        public static async Task<ProjectTestDatabase> CreateAsync()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var options = new DbContextOptionsBuilder<PortfolioDbContext>()
                .UseSqlite(connection)
                .Options;
            var context = new PortfolioDbContext(options);
            await context.Database.EnsureCreatedAsync();
            return new ProjectTestDatabase(connection, context);
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }
}
