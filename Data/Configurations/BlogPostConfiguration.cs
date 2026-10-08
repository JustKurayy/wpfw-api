using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WpfwApi.Models;

namespace WpfwApi.Data.Configurations;

public sealed class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
{
    public void Configure(EntityTypeBuilder<BlogPost> builder)
    {
        builder.HasKey(blogPost => blogPost.Id);
        builder.Property(blogPost => blogPost.Titel).HasMaxLength(200).IsRequired();
        builder.Property(blogPost => blogPost.Inhoud).HasMaxLength(20_000).IsRequired();
        builder.Property(blogPost => blogPost.Publicatiedatum).HasColumnType("datetime2");
    }
}
