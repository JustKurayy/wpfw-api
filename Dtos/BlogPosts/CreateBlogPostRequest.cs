using System.ComponentModel.DataAnnotations;

namespace WpfwApi.Dtos.BlogPosts;

public sealed class CreateBlogPostRequest
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public required string Titel { get; init; }

    [Required]
    [StringLength(20_000, MinimumLength = 10)]
    public required string Inhoud { get; init; }

    public DateTime Publicatiedatum { get; init; }
}
