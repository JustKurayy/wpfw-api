using Microsoft.AspNetCore.Mvc;
using WpfwApi.Controllers;
using WpfwApi.Dtos.BlogPosts;
using WpfwApi.Services.BlogPosts;

namespace WpfwApi.Tests.BlogPosts;

public sealed class BlogPostsControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsOkWithBlogPosts()
    {
        var service = new BlogPostServiceStub { Items = [Response(1)] };
        var result = await new BlogPostsController(service).GetAll(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Single(Assert.IsAssignableFrom<IReadOnlyList<BlogPostResponse>>(ok.Value));
    }

    [Fact]
    public async Task GetById_ReturnsBlogPostOrNotFound()
    {
        var service = new BlogPostServiceStub { Item = Response(7) };
        var controller = new BlogPostsController(service);

        var found = await controller.GetById(7, CancellationToken.None);
        service.Item = null;
        var missing = await controller.GetById(99, CancellationToken.None);

        Assert.IsType<OkObjectResult>(found.Result);
        Assert.IsType<NotFoundResult>(missing.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtGetById()
    {
        var service = new BlogPostServiceStub { Item = Response(4) };
        var result = await new BlogPostsController(service).Create(CreateRequest(), CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(BlogPostsController.GetById), created.ActionName);
        Assert.Equal(4, created.RouteValues!["id"]);
    }

    [Fact]
    public async Task Update_ReturnsOkOrNotFound()
    {
        var service = new BlogPostServiceStub { Item = Response(5) };
        var controller = new BlogPostsController(service);

        var found = await controller.Update(5, UpdateRequest(), CancellationToken.None);
        service.Item = null;
        var missing = await controller.Update(99, UpdateRequest(), CancellationToken.None);

        Assert.IsType<OkObjectResult>(found.Result);
        Assert.IsType<NotFoundResult>(missing.Result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContentOrNotFound()
    {
        var service = new BlogPostServiceStub { Deleted = true };
        var controller = new BlogPostsController(service);

        var deleted = await controller.Delete(5, CancellationToken.None);
        service.Deleted = false;
        var missing = await controller.Delete(99, CancellationToken.None);

        Assert.IsType<NoContentResult>(deleted);
        Assert.IsType<NotFoundResult>(missing);
    }

    private static BlogPostResponse Response(int id) => new(
        id,
        "Eerste blogpost",
        "Dit is de inhoud van mijn eerste blogpost.",
        new DateTime(2026, 10, 26, 10, 0, 0, DateTimeKind.Utc));

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

    private sealed class BlogPostServiceStub : IBlogPostService
    {
        public IReadOnlyList<BlogPostResponse> Items { get; set; } = [];
        public BlogPostResponse? Item { get; set; }
        public bool Deleted { get; set; }

        public Task<IReadOnlyList<BlogPostResponse>> GetAllAsync(CancellationToken cancellationToken) =>
            Task.FromResult(Items);

        public Task<BlogPostResponse?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
            Task.FromResult(Item);

        public Task<BlogPostResponse> CreateAsync(CreateBlogPostRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(Item!);

        public Task<BlogPostResponse?> UpdateAsync(
            int id,
            UpdateBlogPostRequest request,
            CancellationToken cancellationToken) => Task.FromResult(Item);

        public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken) => Task.FromResult(Deleted);
    }
}
