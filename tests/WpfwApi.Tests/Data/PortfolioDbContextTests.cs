using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WpfwApi.Data;
using WpfwApi.Models;
using WpfwApi.Tests.Infrastructure;

namespace WpfwApi.Tests.Data;

public sealed class PortfolioDbContextTests
{
    [Fact]
    public void Model_ConfiguresProjectAndBlogPostConstraints()
    {
        var options = new DbContextOptionsBuilder<PortfolioDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ModelOnly;Trusted_Connection=True")
            .Options;
        using var context = new PortfolioDbContext(options);

        var project = context.Model.FindEntityType(typeof(Project));
        var blogPost = context.Model.FindEntityType(typeof(BlogPost));

        Assert.NotNull(project);
        Assert.Equal(nameof(Project.Id), Assert.Single(project.FindPrimaryKey()!.Properties).Name);
        Assert.False(project.FindProperty(nameof(Project.Titel))!.IsNullable);
        Assert.Equal(200, project.FindProperty(nameof(Project.Titel))!.GetMaxLength());
        Assert.Equal(2_000, project.FindProperty(nameof(Project.Beschrijving))!.GetMaxLength());
        Assert.Equal(100, project.FindProperty(nameof(Project.Categorie))!.GetMaxLength());
        Assert.Equal(2_048, project.FindProperty(nameof(Project.GitHubUrl))!.GetMaxLength());

        Assert.NotNull(blogPost);
        Assert.Equal(nameof(BlogPost.Id), Assert.Single(blogPost.FindPrimaryKey()!.Properties).Name);
        Assert.False(blogPost.FindProperty(nameof(BlogPost.Titel))!.IsNullable);
        Assert.Equal(200, blogPost.FindProperty(nameof(BlogPost.Titel))!.GetMaxLength());
        Assert.Equal(20_000, blogPost.FindProperty(nameof(BlogPost.Inhoud))!.GetMaxLength());
    }

    [Fact]
    public void Application_RegistersSqlServerPortfolioDbContext()
    {
        using var factory = new ApiFactory();
        using var scope = factory.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();

        Assert.Equal("Microsoft.EntityFrameworkCore.SqlServer", context.Database.ProviderName);
    }
}
