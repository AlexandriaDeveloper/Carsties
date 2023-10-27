using AuctionService.Data;
using Contracts;
using MassTransit;

namespace AuctionService;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    private readonly AuctionDbContexct _dbContexct;

    public BidPlacedConsumer(AuctionDbContexct dbContexct)
    {
        this._dbContexct = dbContexct;


    }
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine("=============>BidPlaced Consumer");
        var auction = await _dbContexct.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId));
        if (auction.CurrentHighBid == null || context.Message.BidStatus.Contains("Accepted") && context.Message.Amount > auction.CurrentHighBid)
        {

            auction.CurrentHighBid = context.Message.Amount;
            await _dbContexct.SaveChangesAsync();
        }

    }
}
