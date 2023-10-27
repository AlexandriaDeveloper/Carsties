using AuctionService.Data;
using Grpc.Core;

namespace AuctionService;
public class GrpcAuctionService : GrpcAuction.GrpcAuctionBase
{
    private readonly AuctionDbContexct _dbContext;

    public GrpcAuctionService(AuctionDbContexct dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<GrpcAuctionResponse> GetAuction(GetAuctionRequest request, ServerCallContext context)
    {
        Console.WriteLine("=============>GetAuction GRPC Request");
        var auction = await _dbContext.Auctions.FindAsync(Guid.Parse(request.Id));
        if (auction == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Auction not found"));
        }

        var rsponse = new GrpcAuctionResponse
        {
            Auction = new GrpcAuctionModel()
            {
                AuctionEnd = auction.AuctionEnd.ToString(),
                Id = auction.Id.ToString(),
                ReservePrice = auction.ReservePrice,
                Seller = auction.Seller,
            }
        };

        return rsponse;
    }
}