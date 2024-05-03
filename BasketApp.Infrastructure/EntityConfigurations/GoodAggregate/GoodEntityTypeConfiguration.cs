using BasketApp.Core.Domain.GoodAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BasketApp.Infrastructure.EntityConfigurations.GoodAggregate;

class GoodEntityTypeConfiguration : IEntityTypeConfiguration<Good>
{
    public void Configure(EntityTypeBuilder<Good> entityTypeBuilder)
    {
        entityTypeBuilder
            .ToTable("goods");

        entityTypeBuilder
            .Property(entity => entity.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        entityTypeBuilder
            .Property(entity => entity.Title)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("title")
            .IsRequired();

        entityTypeBuilder
            .Property(entity => entity.Description)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("description")
            .IsRequired();

        entityTypeBuilder
            .Property(entity => entity.Price)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("price")
            .IsRequired();

        entityTypeBuilder
            .OwnsOne(entity => entity.Weight, w =>
            {
                w.Property(v => v.Value).HasColumnName("weight_gram");
                w.WithOwner();
            });
    }
}