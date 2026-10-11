using System.ComponentModel.DataAnnotations;

namespace WpfwApi.Dtos.Projects;

public sealed class CreateProjectRequest
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public required string Titel { get; init; }

    [Required]
    [StringLength(2_000, MinimumLength = 10)]
    public required string Beschrijving { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public required string Categorie { get; init; }

    [Required]
    [StringLength(2_048)]
    [Url]
    public required string GitHubUrl { get; init; }

    public DateTime Datum { get; init; }
}
