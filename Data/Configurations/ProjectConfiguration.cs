using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WpfwApi.Models;

namespace WpfwApi.Data.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(project => project.Id);
        builder.Property(project => project.Titel).HasMaxLength(200).IsRequired();
        builder.Property(project => project.Beschrijving).HasMaxLength(2_000).IsRequired();
        builder.Property(project => project.Categorie).HasMaxLength(100).IsRequired();
        builder.Property(project => project.GitHubUrl).HasMaxLength(2_048).IsRequired();
        builder.Property(project => project.Datum).HasColumnType("date");
    }
}
