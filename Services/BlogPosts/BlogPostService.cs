using Microsoft.EntityFrameworkCore;
using WpfwApi.Data;
using WpfwApi.Dtos.BlogPosts;
using WpfwApi.Models;

namespace WpfwApi.Services.BlogPosts;

public sealed class BlogPostService(PortfolioDbContext dbContext) : IBlogPostService
{
    public async Task<IReadOnlyList<BlogPostResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.BlogPosts
            .AsNoTracking()
            .OrderBy(blogPost => blogPost.Id)
            .Select(blogPost => ToResponse(blogPost))
            .ToListAsync(cancellationToken);
    }

    public async Task<BlogPostResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.BlogPosts
            .AsNoTracking()
            .Where(blogPost => blogPost.Id == id)
            .Select(blogPost => ToResponse(blogPost))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<BlogPostResponse> CreateAsync(
        CreateBlogPostRequest request,
        CancellationToken cancellationToken)
    {
        var blogPost = new BlogPost
        {
            Titel = request.Titel,
            Inhoud = request.Inhoud,
            Publicatiedatum = request.Publicatiedatum
        };
        dbContext.BlogPosts.Add(blogPost);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(blogPost);
    }

    public async Task<BlogPostResponse?> UpdateAsync(
        int id,
        UpdateBlogPostRequest request,
        CancellationToken cancellationToken)
    {
        var blogPost = await dbContext.BlogPosts.FindAsync([id], cancellationToken);
        if (blogPost is null)
        {
            return null;
        }

        blogPost.Titel = request.Titel;
        blogPost.Inhoud = request.Inhoud;
        blogPost.Publicatiedatum = request.Publicatiedatum;
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(blogPost);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var blogPost = await dbContext.BlogPosts.FindAsync([id], cancellationToken);
        if (blogPost is null)
        {
            return false;
        }

        dbContext.BlogPosts.Remove(blogPost);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static BlogPostResponse ToResponse(BlogPost blogPost) => new(
        blogPost.Id,
        blogPost.Titel,
        blogPost.Inhoud,
        blogPost.Publicatiedatum);
}
