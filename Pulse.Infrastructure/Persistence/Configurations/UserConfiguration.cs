using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulse.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .HasColumnName("id");

            builder.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(320)
                .HasColumnName("email");

            builder.Property(t => t.PasswordHash)
                .IsRequired()
                .HasColumnName("password_hash");

            builder.Property(t => t.DisplayName)
                .IsRequired()
                .HasColumnName("display_name");

            builder.Property(t => t.CreatedAt)
                .IsRequired()
                .HasColumnName("created_at");

            builder.Property(t => t.UpdatedAt)
                .IsRequired()
                .HasColumnName("updated_at");

            builder.HasIndex(x => x.Email)
                .IsUnique();

            builder.Property(x => x.Role)
                .HasColumnName("role")
                .HasConversion<string>()
                .IsRequired();
        }
    }
}
