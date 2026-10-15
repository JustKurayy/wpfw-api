using System.Net;
using System.Net.Http.Json;
using WpfwApi.Dtos.BlogPosts;
using WpfwApi.Dtos.Projects;
using WpfwApi.Tests.Infrastructure;

namespace WpfwApi.Tests.Integration;

public sealed class SqlServerCrudTests
{
    [Fact]
    [Trait("Category", "SqlServer")]
    public async Task ProjectAndBlogPost_CrudSurvivesApplicationRestart()
    {
        var databaseName = $"WpfwPortfolioTests_{Guid.NewGuid():N}";
        var connectionString = $"Server=localhost\\SQLEXPRESS;Database={databaseName};Trusted_Connection=True;TrustServerCertificate=True";
        ApiFactory? activeFactory = null;

        try
        {
            activeFactory = new ApiFactory(connectionString);
            await activeFactory.MigrateDatabaseAsync();
            using var firstClient = activeFactory.CreateClient();

            var project = await CreateProjectAsync(firstClient);
            var blogPost = await CreateBlogPostAsync(firstClient);
            await AssertProjectGetRoutesAsync(firstClient, project.Id);
            await AssertBlogPostGetRoutesAsync(firstClient, blogPost.Id);
            await UpdateProjectAsync(firstClient, project.Id);
            await UpdateBlogPostAsync(firstClient, blogPost.Id);

            activeFactory.Dispose();
            activeFactory = new ApiFactory(connectionString);
            using var restartedClient = activeFactory.CreateClient();

            var persistedProject = await restartedClient.GetFromJsonAsync<ProjectResponse>($"/api/projects/{project.Id}");
            var persistedBlogPost = await restartedClient.GetFromJsonAsync<BlogPostResponse>($"/api/blogposts/{blogPost.Id}");
            Assert.Equal("Portfolio API bijgewerkt", persistedProject!.Titel);
            Assert.Equal("Blogpost bijgewerkt", persistedBlogPost!.Titel);

            Assert.Equal(HttpStatusCode.NoContent, (await restartedClient.DeleteAsync($"/api/projects/{project.Id}")).StatusCode);
            Assert.Equal(HttpStatusCode.NoContent, (await restartedClient.DeleteAsync($"/api/blogposts/{blogPost.Id}")).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await restartedClient.GetAsync($"/api/projects/{project.Id}")).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await restartedClient.GetAsync($"/api/blogposts/{blogPost.Id}")).StatusCode);
        }
        finally
        {
            if (activeFactory is not null)
            {
                await activeFactory.DeleteDatabaseAsync();
                activeFactory.Dispose();
            }
        }
    }

    private static async Task<ProjectResponse> CreateProjectAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/projects", new CreateProjectRequest
        {
            Titel = "Portfolio API",
            Beschrijving = "Backend voor mijn portfolio",
            Categorie = "Backend",
            GitHubUrl = "https://github.com/voorbeeld/portfolio-api",
            Datum = new DateTime(2026, 10, 26)
        });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<ProjectResponse>())!;
    }

    private static async Task<BlogPostResponse> CreateBlogPostAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/blogposts", new CreateBlogPostRequest
        {
            Titel = "Eerste blogpost",
            Inhoud = "Dit is de inhoud van mijn eerste blogpost.",
            Publicatiedatum = new DateTime(2026, 10, 26, 10, 0, 0, DateTimeKind.Utc)
        });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<BlogPostResponse>())!;
    }

    private static async Task AssertProjectGetRoutesAsync(HttpClient client, int id)
    {
        Assert.Contains((await client.GetFromJsonAsync<ProjectResponse[]>("/api/projects"))!, project => project.Id == id);
        Assert.Equal(id, (await client.GetFromJsonAsync<ProjectResponse>($"/api/projects/{id}"))!.Id);
    }

    private static async Task AssertBlogPostGetRoutesAsync(HttpClient client, int id)
    {
        Assert.Contains((await client.GetFromJsonAsync<BlogPostResponse[]>("/api/blogposts"))!, post => post.Id == id);
        Assert.Equal(id, (await client.GetFromJsonAsync<BlogPostResponse>($"/api/blogposts/{id}"))!.Id);
    }

    private static async Task UpdateProjectAsync(HttpClient client, int id)
    {
        var response = await client.PutAsJsonAsync($"/api/projects/{id}", new UpdateProjectRequest
        {
            Titel = "Portfolio API bijgewerkt",
            Beschrijving = "De bijgewerkte backend voor mijn portfolio",
            Categorie = "C#",
            GitHubUrl = "https://github.com/voorbeeld/portfolio-api",
            Datum = new DateTime(2026, 10, 27)
        });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private static async Task UpdateBlogPostAsync(HttpClient client, int id)
    {
        var response = await client.PutAsJsonAsync($"/api/blogposts/{id}", new UpdateBlogPostRequest
        {
            Titel = "Blogpost bijgewerkt",
            Inhoud = "Dit is de bijgewerkte inhoud van mijn blogpost.",
            Publicatiedatum = new DateTime(2026, 10, 27, 10, 0, 0, DateTimeKind.Utc)
        });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
