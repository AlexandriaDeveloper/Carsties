
using AuctionService.DTOs;
using AuctionService.Entitles;
using AutoMapper;
using Contracts;

namespace AuctionService.RequestHelpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Item);

            CreateMap<Item, AuctionDto>();

            CreateMap<CreeateAuctionDto, Auction>()
            .ForMember(d => d.Item, o => o.MapFrom(s => s));

            CreateMap<CreeateAuctionDto, Item>();

            CreateMap<AuctionDto, AuctionCreated>();


            CreateMap<Auction, AuctionUpdated>().IncludeMembers(x => x.Item);
            CreateMap<Item, AuctionUpdated>();
        }
    }
}