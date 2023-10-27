using AuctionService.Data;
using AuctionService.Entitles;
using Contracts;
using MassTransit;

namespace AuctionService;

public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
{
    private readonly AuctionDbContexct _contexct;
    public AuctionFinishedConsumer(AuctionDbContexct contexct)
    {
        this._contexct = contexct;

    }
    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
        System.Console.WriteLine("=============>AuctionFinished Consumer");
        var auction = await _contexct.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId));
        if (context.Message.ItemSold)
        {
            auction.Winner = context.Message.Winner;
            auction.SoldAmount = (int)context.Message.Amount;

        }
        auction.Status = auction.SoldAmount > auction.ReservePrice
        ? Status.Finished : Status.ReserveNotMet;
        await _contexct.SaveChangesAsync();


    }
}
