using WpfwApi.Dtos.Projects;

namespace WpfwApi.Services.Projects;

public interface IProjectService
{
    Task<IReadOnlyList<ProjectResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<ProjectResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ProjectResponse> CreateAsync(CreateProjectRequest request, CancellationToken cancellationToken);
    Task<ProjectResponse?> UpdateAsync(int id, UpdateProjectRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
