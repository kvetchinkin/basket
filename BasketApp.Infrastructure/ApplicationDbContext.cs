using BasketApp.Core.Domain.BasketAggregate;
using BasketApp.Core.Domain.GoodAggregate;
using BasketApp.Infrastructure.Entities;
using BasketApp.Infrastructure.EntityConfigurations.BasketAggregate;
using BasketApp.Infrastructure.EntityConfigurations.GoodAggregate;
using BasketApp.Infrastructure.EntityConfigurations.Outbox;
using Microsoft.EntityFrameworkCore;

namespace BasketApp.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public DbSet<Basket> Baskets { get; set; }
    public DbSet<Good> Goods { get; set; }
    
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Basket
        modelBuilder.ApplyConfiguration(new BasketEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ItemEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new TimeSlotEntityTypeConfiguration());

        //TimeSlot
        modelBuilder.Entity<TimeSlot>(b =>
        {
            var allTimeSlots = TimeSlot.List();
            b.HasData(allTimeSlots.Select(c => new {c.Id, c.Name, c.Start, c.End}));
        });


        //Good Aggregate
        modelBuilder.ApplyConfiguration(new GoodEntityTypeConfiguration());

        //Goods
        modelBuilder.Entity<Good>(b =>
        {
            var allGoods = Good.List();
            b.HasData(allGoods.Select(c => new {c.Id, c.Title, c.Description, c.Price}));
            b.OwnsOne(e => e.Weight).HasData(allGoods.Select(c => new {GoodId = c.Id, c.Weight.Value}));
        });

        //Outbox
        modelBuilder.ApplyConfiguration(new OutboxEntityTypeConfiguration());
    }
}