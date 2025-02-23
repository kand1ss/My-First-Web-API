using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
{
    public void Configure(EntityTypeBuilder<UserAccount> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(x => x.ExternalId).IsRequired().HasMaxLength(36);
        builder.Property(x => x.Login).IsRequired().HasMaxLength(24);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(48);
        builder.Property(x => x.FirstName).HasMaxLength(32);
        builder.Property(x => x.LastName).HasMaxLength(32);
        builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(128);
        
        builder.HasIndex(x => x.ExternalId);
    }
}