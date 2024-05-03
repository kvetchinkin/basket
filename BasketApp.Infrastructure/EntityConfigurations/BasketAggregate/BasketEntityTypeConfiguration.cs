using BasketApp.Core.Domain.BasketAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BasketApp.Infrastructure.EntityConfigurations.BasketAggregate;

class BasketEntityTypeConfiguration : IEntityTypeConfiguration<Basket>
{
    public void Configure(EntityTypeBuilder<Basket> entityTypeBuilder)
    {
        entityTypeBuilder.ToTable("baskets");

        entityTypeBuilder.HasKey(entity => entity.Id);

        entityTypeBuilder.Property(entity => entity.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        var navigation = entityTypeBuilder.Metadata.FindNavigation(nameof(Basket.Items));

        entityTypeBuilder
            .OwnsOne(entity => entity.Address, a =>
            {
                a.Property(c => c.Country).HasColumnName("address_country").IsRequired(false);
                a.Property(c => c.City).HasColumnName("address_city").IsRequired(false);
                a.Property(c => c.Street).HasColumnName("address_street").IsRequired(false);
                a.Property(c => c.House).HasColumnName("address_house").IsRequired(false);
                a.Property(c => c.Apartment).HasColumnName("address_apartment").IsRequired(false);
                a.WithOwner();
            });
        
        entityTypeBuilder
            .OwnsOne(entity => entity.Status, a =>
            {
                a.Property(c => c.Value).HasColumnName("status").IsRequired();
                a.WithOwner();
            });

        entityTypeBuilder.HasOne(entity => entity.TimeSlot)
            .WithMany()
            .IsRequired(false)
            .HasForeignKey("timeslot_id");
    }
}