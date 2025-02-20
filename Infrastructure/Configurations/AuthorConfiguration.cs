using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(32);
        builder.Property(x => x.LastName).IsRequired().HasMaxLength(32);
        
        builder
            .HasMany(x => x.Books)
            .WithOne(x => x.Author)
            .HasForeignKey(x => x.AuthorId)
            .IsRequired();
    }
}