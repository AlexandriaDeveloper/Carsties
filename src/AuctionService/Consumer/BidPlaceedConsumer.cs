using AuctionService.Data;
using Contracts;
using MassTransit;

namespace AuctionService;

public class BidPlaceedConsumer : IConsumer<BidPlaced>
{
    private readonly AuctionDbContexct _dbContexct;

    public BidPlaceedConsumer(AuctionDbContexct dbContexct)
    {
        this._dbContexct = dbContexct;


    }
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine("=============>BidPlaced Consumer");
        var auction = await _dbContexct.Auctions.FindAsync(context.Message.AuctionId);
        if (!auction.CurrentHighBid.HasValue || context.Message.BidStatus.Contains("Accepted") && context.Message.Amount > auction.CurrentHighBid)
        {

            auction.CurrentHighBid = context.Message.Amount;
            await _dbContexct.SaveChangesAsync();
        }

    }
}
