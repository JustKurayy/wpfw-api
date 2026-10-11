namespace WpfwApi.Dtos.Projects;

public sealed record ProjectResponse(
    int Id,
    string Titel,
    string Beschrijving,
    string Categorie,
    string GitHubUrl,
    DateTime Datum);
