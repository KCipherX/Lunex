using Lunex.Domain.Identity.Aggregates;
using Lunex.Domain.Identity.ValueObjects;
using Lunex.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Lunex.Infrastructure.Persistance.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ToTable("Users", Constants.Schemas.Application);

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasMaxLength(36);

        var emailConverter = new ValueConverter<Email, string>(
            v => v.Value,
            v => Email.Create(v));

        var passwordHashConverter = new ValueConverter<PasswordHash, string>(
            v => v.Value,
            v => PasswordHash.Create(v));

        var passwordSaltConverter = new ValueConverter<PasswordSalt, string>(
            v => v.Value,
            v => PasswordSalt.Create(v));

        entity.Property(e => e.Email)
            .HasConversion(emailConverter)
            .HasColumnName("Email")
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(e => e.PasswordHash)
            .HasConversion(passwordHashConverter)
            .HasColumnName("PasswordHash")
            .HasMaxLength(512)
            .IsRequired();

        entity.Property(e => e.PasswordSalt)
            .HasConversion(passwordSaltConverter)
            .HasColumnName("PasswordSalt")
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(e => e.Name).HasMaxLength(100);
        entity.Property(e => e.ImageUrl).HasMaxLength(500);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.LastLoginAt);
        entity.Ignore(e => e.DomainEvents);

        entity.HasIndex(e => e.Email).IsUnique();
    }
}