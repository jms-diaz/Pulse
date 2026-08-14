using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulse.Infrastructure.Persistence.Configurations
{
    public sealed class RefreshTokenConfiguration
    : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(
            EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("refresh_tokens");

            builder.HasKey(x => x.Id);

            builder.Property(t => t.Id)
                .HasColumnName("id");

            builder.Property(x => x.UserId)
                .IsRequired()
                .HasColumnName("user_id");

            builder.Property(x => x.TokenHash)
                .IsRequired()
                .HasColumnName("token_hash");

            builder.Property(x => x.ExpiresAt)
                .IsRequired()
                .HasColumnName("expires_at");

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasColumnName("created_at");

            builder.Property(x => x.RevokedAt)
                .IsRequired(false)
                .HasColumnName("revoked_at");

            builder.HasIndex(x => x.TokenHash)
                .IsUnique();

            builder.HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
