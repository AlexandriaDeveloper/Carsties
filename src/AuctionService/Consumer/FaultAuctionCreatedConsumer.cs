
using AuctionService.Data;
using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuctionService
{
    public class FaultAuctionCreatedConsumer : IConsumer<Fault<AuctionCreated>>
    {
        private readonly AuctionDbContexct _context;

        public FaultAuctionCreatedConsumer(AuctionDbContexct context)
        {
            _context = context;
        }
        public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
        {

            var exception = context.Message.Exceptions.First();

            if (exception.ExceptionType == "System.TimeoutException")
            {
                Console.WriteLine("=============>Exception While Saving ====>", context.Message.Exceptions.First().Message);

                var auctionToDelete = await _context.Auctions
                .Include(a => a.Item)
                .FirstOrDefaultAsync(x => x.Id == context.Message.Message.Id);
                if (auctionToDelete != null)
                {
                    _context.Remove(auctionToDelete);
                    _context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Couldnt delete====>", context.Message);
                }

            }




        }
    }
}