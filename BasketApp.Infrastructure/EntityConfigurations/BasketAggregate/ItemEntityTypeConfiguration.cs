using BasketApp.Core.Domain.BasketAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BasketApp.Infrastructure.EntityConfigurations.BasketAggregate;

class ItemEntityTypeConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> entityTypeBuilder)
    {
        entityTypeBuilder
            .ToTable("items");

        entityTypeBuilder
            .HasKey(entity => entity.Id);

        entityTypeBuilder
            .Property(entity => entity.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        entityTypeBuilder
            .Property(entity => entity.GoodId)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("good_id")
            .IsRequired();

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
            .Property("BasketId")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("basket_id")
            .IsRequired();

        entityTypeBuilder
            .Property(entity => entity.Price)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("price")
            .IsRequired();

        entityTypeBuilder
            .Property(entity => entity.Quantity)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("quantity")
            .IsRequired();
    }
}