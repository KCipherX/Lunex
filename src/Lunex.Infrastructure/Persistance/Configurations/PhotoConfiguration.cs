using Lunex.Domain.Member.Aggregates;
using Lunex.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lunex.Infrastructure.Persistance.Configurations;

public sealed class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> entity)
    {
        entity.ToTable("Photos", Constants.Schemas.Application);

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasMaxLength(36);
        entity.Property(e => e.Url).HasMaxLength(500).IsRequired();
        entity.Property(e => e.PublicId).HasMaxLength(200);
        entity.Property(e => e.IsMain).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();
    }
}