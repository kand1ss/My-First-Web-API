using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class RefreshTokensConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder
            .HasOne<UserAccount>()
            .WithOne()
            .HasForeignKey<RefreshToken>(x => x.UserId)
            .HasPrincipalKey<UserAccount>(x => x.ExternalId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(x => x.Token)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(x => x.UserId);
    }
}