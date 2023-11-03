using AutoMapper;
using Contracts;

namespace BiddingService;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Bid, BidDto>();
        CreateMap<Bid, BidPlaced>()
        //.ForMember(dest => dest.BidStatus, opt => opt.MapFrom(src => src.Status.ToString()))
        ;
    }
}
