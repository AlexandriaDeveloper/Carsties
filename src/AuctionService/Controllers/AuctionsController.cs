using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entitles;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionsController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IAuctionRepository _repo;

        public AuctionsController(IAuctionRepository repo, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            this._repo = repo;

            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }
        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuction(string date)
        {
            return await _repo.GetAuctionsAsync(date);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
        {
            var auction = await _repo.GetAuctionByIdAsync(id);
            if (auction == null)
            {
                return NotFound();
            }
            return auction;
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
        {
            var auction = _mapper.Map<Auction>(auctionDto);
            //TODO :add Current user as seller 

            auction.Seller = User.Identity.Name;
            _repo.AddAuction(auction);
            var newAuction = _mapper.Map<AuctionDto>(auction);
            await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));
            var result = await _repo.SaveChangesAsync();

            if (!result)
            {
                return BadRequest("Failed to create auction");
            }
            return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, newAuction);

        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
        {
            Console.WriteLine(updateAuctionDto);

            var auction = await _repo.GetAuctionEntityByIdAsync(id);
            if (auction == null) return NotFound();

            if (auction.Seller != User.Identity.Name) return Forbid();

            auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
            auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

            await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));
            var result = await _repo.SaveChangesAsync();
            if (result) return Ok();

            return BadRequest("Problem saving changes");
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _repo.GetAuctionEntityByIdAsync(id);
            if (auction == null) return NotFound();
            //TODO : check if Current user as seller
            if (auction.Seller != User.Identity.Name) return Forbid();

            _repo.RemoveAuction(auction);
            await _publishEndpoint.Publish<AuctionDeleted>(new { id = auction.Id.ToString() });
            var result = await _repo.SaveChangesAsync();
            if (!result) return BadRequest("Failed to delete auction");
            return Ok();
        }
    }
}