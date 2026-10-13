using Microsoft.AspNetCore.Mvc;
using WpfwApi.Dtos.BlogPosts;
using WpfwApi.Services.BlogPosts;

namespace WpfwApi.Controllers;

[ApiController]
[Route("api/blogposts")]
public sealed class BlogPostsController(IBlogPostService blogPostService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<BlogPostResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BlogPostResponse>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await blogPostService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}", Name = "GetBlogPostById")]
    [ProducesResponseType<BlogPostResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BlogPostResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var blogPost = await blogPostService.GetByIdAsync(id, cancellationToken);
        return blogPost is null ? NotFound() : Ok(blogPost);
    }

    [HttpPost]
    [ProducesResponseType<BlogPostResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BlogPostResponse>> Create(
        CreateBlogPostRequest request,
        CancellationToken cancellationToken)
    {
        var blogPost = await blogPostService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = blogPost.Id }, blogPost);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<BlogPostResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BlogPostResponse>> Update(
        int id,
        UpdateBlogPostRequest request,
        CancellationToken cancellationToken)
    {
        var blogPost = await blogPostService.UpdateAsync(id, request, cancellationToken);
        return blogPost is null ? NotFound() : Ok(blogPost);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await blogPostService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
    }
}
