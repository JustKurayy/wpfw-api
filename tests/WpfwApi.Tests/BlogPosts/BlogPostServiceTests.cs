using System.ComponentModel.DataAnnotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WpfwApi.Data;
using WpfwApi.Dtos.BlogPosts;
using WpfwApi.Services.BlogPosts;

namespace WpfwApi.Tests.BlogPosts;

public sealed class BlogPostServiceTests
{
    [Fact]
    public void CreateRequest_RejectsWhitespaceTitle_AndDoesNotExposeId()
    {
        var request = new CreateBlogPostRequest
        {
            Titel = " ",
            Inhoud = "Dit is een geldige blogtekst.",
            Publicatiedatum = new DateTime(2026, 10, 26, 10, 0, 0, DateTimeKind.Utc)
        };
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(request, new ValidationContext(request), results, true);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(request.Titel)));
        Assert.Null(typeof(CreateBlogPostRequest).GetProperty("Id"));
    }

    [Fact]
    public async Task CrudMethods_PersistReadUpdateAndDeleteBlogPost()
    {
        await using var database = await BlogPostTestDatabase.CreateAsync();
        var service = new BlogPostService(database.Context);

        var created = await service.CreateAsync(CreateRequest(), CancellationToken.None);
        var all = await service.GetAllAsync(CancellationToken.None);
        var single = await service.GetByIdAsync(created.Id, CancellationToken.None);
        var changed = await service.UpdateAsync(created.Id, UpdateRequest(), CancellationToken.None);
        var deleted = await service.DeleteAsync(created.Id, CancellationToken.None);

        Assert.True(created.Id > 0);
        Assert.Single(all);
        Assert.Equal(created, single);
        Assert.Equal("Bijgewerkte blogpost", changed!.Titel);
        Assert.True(deleted);
        Assert.Null(await service.GetByIdAsync(created.Id, CancellationToken.None));
    }

    [Fact]
    public async Task MissingId_ReturnsNullOrFalse()
    {
        await using var database = await BlogPostTestDatabase.CreateAsync();
        var service = new BlogPostService(database.Context);

        Assert.Null(await service.GetByIdAsync(42, CancellationToken.None));
        Assert.Null(await service.UpdateAsync(42, UpdateRequest(), CancellationToken.None));
        Assert.False(await service.DeleteAsync(42, CancellationToken.None));
    }

    private static CreateBlogPostRequest CreateRequest() => new()
    {
        Titel = "Eerste blogpost",
        Inhoud = "Dit is de inhoud van mijn eerste blogpost.",
        Publicatiedatum = new DateTime(2026, 10, 26, 10, 0, 0, DateTimeKind.Utc)
    };

    private static UpdateBlogPostRequest UpdateRequest() => new()
    {
        Titel = "Bijgewerkte blogpost",
        Inhoud = "Dit is de bijgewerkte inhoud van mijn blogpost.",
        Publicatiedatum = new DateTime(2026, 10, 27, 10, 0, 0, DateTimeKind.Utc)
    };

    private sealed class BlogPostTestDatabase : IAsyncDisposable
    {
        private readonly SqliteConnection _connection;

        private BlogPostTestDatabase(SqliteConnection connection, PortfolioDbContext context)
        {
            _connection = connection;
            Context = context;
        }

        public PortfolioDbContext Context { get; }

        public static async Task<BlogPostTestDatabase> CreateAsync()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var options = new DbContextOptionsBuilder<PortfolioDbContext>().UseSqlite(connection).Options;
            var context = new PortfolioDbContext(options);
            await context.Database.EnsureCreatedAsync();
            return new BlogPostTestDatabase(connection, context);
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }
}
