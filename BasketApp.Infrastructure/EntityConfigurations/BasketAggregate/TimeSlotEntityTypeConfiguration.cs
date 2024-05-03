using BasketApp.Core.Domain.BasketAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BasketApp.Infrastructure.EntityConfigurations.BasketAggregate;

class TimeSlotEntityTypeConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> entityTypeBuilder)
    {
        entityTypeBuilder
            .ToTable("time_slots");

        entityTypeBuilder
            .HasKey(entity => entity.Id);

        entityTypeBuilder
            .Property(entity => entity.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        entityTypeBuilder
            .Property(entity => entity.Name)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("name")
            .IsRequired();

        entityTypeBuilder
            .Property(entity => entity.Start)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("start")
            .IsRequired();

        entityTypeBuilder
            .Property(entity => entity.End)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("end")
            .IsRequired();
    }
}