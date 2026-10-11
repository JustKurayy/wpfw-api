using Microsoft.AspNetCore.Mvc;
using WpfwApi.Controllers;
using WpfwApi.Dtos.Projects;
using WpfwApi.Services.Projects;

namespace WpfwApi.Tests.Projects;

public sealed class ProjectsControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsOkWithProjects()
    {
        var service = new ProjectServiceStub { Items = [Response(1)] };
        var controller = new ProjectsController(service);

        var result = await controller.GetAll(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Single(Assert.IsAssignableFrom<IReadOnlyList<ProjectResponse>>(ok.Value));
    }

    [Fact]
    public async Task GetById_ReturnsProjectOrNotFound()
    {
        var service = new ProjectServiceStub { Item = Response(7) };
        var controller = new ProjectsController(service);

        var found = await controller.GetById(7, CancellationToken.None);
        service.Item = null;
        var missing = await controller.GetById(99, CancellationToken.None);

        Assert.IsType<OkObjectResult>(found.Result);
        Assert.IsType<NotFoundResult>(missing.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtGetById()
    {
        var service = new ProjectServiceStub { Item = Response(4) };
        var controller = new ProjectsController(service);

        var result = await controller.Create(CreateRequest(), CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(ProjectsController.GetById), created.ActionName);
        Assert.Equal(4, created.RouteValues!["id"]);
        Assert.Equal(4, Assert.IsType<ProjectResponse>(created.Value).Id);
    }

    [Fact]
    public async Task Update_ReturnsOkOrNotFound()
    {
        var service = new ProjectServiceStub { Item = Response(5) };
        var controller = new ProjectsController(service);

        var found = await controller.Update(5, UpdateRequest(), CancellationToken.None);
        service.Item = null;
        var missing = await controller.Update(99, UpdateRequest(), CancellationToken.None);

        Assert.IsType<OkObjectResult>(found.Result);
        Assert.IsType<NotFoundResult>(missing.Result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContentOrNotFound()
    {
        var service = new ProjectServiceStub { Deleted = true };
        var controller = new ProjectsController(service);

        var deleted = await controller.Delete(5, CancellationToken.None);
        service.Deleted = false;
        var missing = await controller.Delete(99, CancellationToken.None);

        Assert.IsType<NoContentResult>(deleted);
        Assert.IsType<NotFoundResult>(missing);
    }

    private static ProjectResponse Response(int id) => new(
        id,
        "Portfolio API",
        "Backend voor mijn portfolio",
        "Backend",
        "https://github.com/voorbeeld/portfolio-api",
        new DateTime(2026, 10, 26));

    private static CreateProjectRequest CreateRequest() => new()
    {
        Titel = "Portfolio API",
        Beschrijving = "Backend voor mijn portfolio",
        Categorie = "Backend",
        GitHubUrl = "https://github.com/voorbeeld/portfolio-api",
        Datum = new DateTime(2026, 10, 26)
    };

    private static UpdateProjectRequest UpdateRequest() => new()
    {
        Titel = "Portfolio API gewijzigd",
        Beschrijving = "Bijgewerkte backend voor mijn portfolio",
        Categorie = "C#",
        GitHubUrl = "https://github.com/voorbeeld/portfolio-api",
        Datum = new DateTime(2026, 10, 27)
    };

    private sealed class ProjectServiceStub : IProjectService
    {
        public IReadOnlyList<ProjectResponse> Items { get; set; } = [];
        public ProjectResponse? Item { get; set; }
        public bool Deleted { get; set; }

        public Task<IReadOnlyList<ProjectResponse>> GetAllAsync(CancellationToken cancellationToken) =>
            Task.FromResult(Items);

        public Task<ProjectResponse?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
            Task.FromResult(Item);

        public Task<ProjectResponse> CreateAsync(CreateProjectRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(Item!);

        public Task<ProjectResponse?> UpdateAsync(
            int id,
            UpdateProjectRequest request,
            CancellationToken cancellationToken) => Task.FromResult(Item);

        public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken) => Task.FromResult(Deleted);
    }
}
