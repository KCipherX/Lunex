using Lunex.Domain.Member.Aggregates;
using Lunex.Domain.Member.ValueObjects;
using Lunex.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Lunex.Infrastructure.Persistance.Configurations;

public sealed class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> entity)
    {
        entity.ToTable("Members", Constants.Schemas.Application);

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasMaxLength(36);
        entity.Property(e => e.UserId).HasMaxLength(36).IsRequired();
        entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
        entity.Property(e => e.Gender).HasConversion<int>().IsRequired();

        var dateOfBirthConverter = new ValueConverter<DateOfBirth, DateOnly>(
            v => v.Value,
            v => DateOfBirth.Create(v));

        var locationConverter = new ValueConverter<Location, string>(
            v => $"{v.City}|{v.Country}",
            v => Location.Create(v.Split('|')[0], v.Split('|').Length > 1 ? v.Split('|')[1] : string.Empty));

        entity.Property(e => e.DateOfBirth)
            .HasConversion(dateOfBirthConverter)
            .HasColumnName("DateOfBirth")
            .HasColumnType("date")
            .IsRequired();

        entity.Property(e => e.Location)
            .HasConversion(locationConverter)
            .HasColumnName("Location")
            .HasMaxLength(200)
            .IsRequired();

        entity.Property(e => e.Description).HasMaxLength(500);
        entity.Property(e => e.ImageUrl).HasMaxLength(500);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.LastActiveAt).IsRequired();
        entity.Ignore(e => e.DomainEvents);

        entity.HasMany(e => e.Photos)
            .WithOne()
            .HasForeignKey("MemberId")
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(e => e.UserId).IsUnique();
    }
}