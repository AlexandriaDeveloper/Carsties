
using AuctionService.Entitles;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data;
public class AuctionDbContexct : DbContext
{
    public AuctionDbContexct(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Auction> Auctions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder
        .AddOutboxStateEntity();
    }
}
