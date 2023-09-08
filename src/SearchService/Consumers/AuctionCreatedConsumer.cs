using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace SearchService.Consumers
{
    public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
    {
        private readonly IMapper _mapper;

        public AuctionCreatedConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            System.Console.WriteLine("====> Consuming Auction Created " + context.Message.Id);
            var item = _mapper.Map<Item>(context.Message);
            try
            {
                System.Console.WriteLine("======================Saving...======================");
                await item.SaveAsync();
            }
            catch (System.TimeoutException ex)
            {
                System.Console.WriteLine("Yo... Timeout");
                throw new TimeoutException("Cant Save Car Something went Wrong");
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Something went wrong");
                throw new ArgumentException("Cant Save Car Something went Wrong", ex.Message);
            }

        }
    }
}