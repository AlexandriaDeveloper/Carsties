
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace SearchService.Consumers
{
    public class AuctionDeletedConsumer : IConsumer<AuctionDeleted>
    {
        public async Task Consume(ConsumeContext<AuctionDeleted> context)
        {
            System.Console.WriteLine("====> Consuming Auction Deleted " + context.Message.Id);
            var result = await DB.DeleteAsync<Item>(context.Message.Id);
            if (!result.IsAcknowledged)
                throw new MessageException(typeof(AuctionDeleted), "Problem Deleteing Auction");
        }
    }
}