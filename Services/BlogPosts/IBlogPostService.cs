using WpfwApi.Dtos.BlogPosts;

namespace WpfwApi.Services.BlogPosts;

public interface IBlogPostService
{
    Task<IReadOnlyList<BlogPostResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<BlogPostResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<BlogPostResponse> CreateAsync(CreateBlogPostRequest request, CancellationToken cancellationToken);
    Task<BlogPostResponse?> UpdateAsync(int id, UpdateBlogPostRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
