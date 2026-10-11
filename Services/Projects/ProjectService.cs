using Microsoft.EntityFrameworkCore;
using WpfwApi.Data;
using WpfwApi.Dtos.Projects;
using WpfwApi.Models;

namespace WpfwApi.Services.Projects;

public sealed class ProjectService(PortfolioDbContext dbContext) : IProjectService
{
    public async Task<IReadOnlyList<ProjectResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Projects
            .AsNoTracking()
            .OrderBy(project => project.Id)
            .Select(project => ToResponse(project))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProjectResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Projects
            .AsNoTracking()
            .Where(project => project.Id == id)
            .Select(project => ToResponse(project))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<ProjectResponse> CreateAsync(
        CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var project = new Project
        {
            Titel = request.Titel,
            Beschrijving = request.Beschrijving,
            Categorie = request.Categorie,
            GitHubUrl = request.GitHubUrl,
            Datum = request.Datum
        };

        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(project);
    }

    public async Task<ProjectResponse?> UpdateAsync(
        int id,
        UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var project = await dbContext.Projects.FindAsync([id], cancellationToken);
        if (project is null)
        {
            return null;
        }

        project.Titel = request.Titel;
        project.Beschrijving = request.Beschrijving;
        project.Categorie = request.Categorie;
        project.GitHubUrl = request.GitHubUrl;
        project.Datum = request.Datum;
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(project);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var project = await dbContext.Projects.FindAsync([id], cancellationToken);
        if (project is null)
        {
            return false;
        }

        dbContext.Projects.Remove(project);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ProjectResponse ToResponse(Project project) => new(
        project.Id,
        project.Titel,
        project.Beschrijving,
        project.Categorie,
        project.GitHubUrl,
        project.Datum);
}
