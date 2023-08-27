
using AuctionService.Entitles;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data;
public class AuctionDbContexct : DbContext
{
    public AuctionDbContexct(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Auction> Auctions { get; set; }

}
