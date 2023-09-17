﻿using AuctionService.Controllers;
using AuctionService.DTOs;
using AuctionService.Entitles;
using AuctionService.RequestHelpers;
using AutoFixture;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AuctionService.UnitTests;

public class AuctionsControllerTests
{
    private readonly Mock<IAuctionRepository> _auctionRepository;
    private readonly Mock<IPublishEndpoint> _publishEndPoint;
    private readonly Fixture _fixture;
    private readonly AuctionsController _controller;
    private readonly IMapper _mapper;

    public AuctionsControllerTests()
    {
        _fixture = new Fixture();
        _auctionRepository = new Mock<IAuctionRepository>();
        _publishEndPoint = new Mock<IPublishEndpoint>();
        var mockMapper = new MapperConfiguration(mc =>
        {
            mc.AddMaps(typeof(MappingProfiles).Assembly);
        }).CreateMapper().ConfigurationProvider;
        _mapper = new Mapper(mockMapper);
        _controller = new AuctionsController(_auctionRepository.Object, _mapper, _publishEndPoint.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = Helpers.GetClaimsPrincipal() }
            }
        };

    }
    [Fact]
    public async Task GetAuctions_WithNoParams_Returns10Auction()
    {
        //arrange
        var auctions = _fixture.CreateMany<AuctionDto>(10).ToList();
        _auctionRepository.Setup(repo => repo.GetAuctionsAsync(null)).ReturnsAsync(auctions);

        //act
        var result = await _controller.GetAllAuction(null);
        //assert
        Assert.Equal(10, result.Value.Count());
        Assert.IsType<ActionResult<List<AuctionDto>>>(result);

    }
    [Fact]
    public async Task GetAuctionById_WithValidGuid_ReturnsAuction()
    {
        //arrange
        var auction = _fixture.Create<AuctionDto>();
        _auctionRepository.Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);

        //act
        var result = await _controller.GetAuctionById(auction.Id);
        //assert
        Assert.Equal(auction.Make, result.Value.Make);
        Assert.IsType<ActionResult<AuctionDto>>(result);

    }



    [Fact]
    public async Task GetAuctionById_WithInvalidGuid_ReturnsNotFound()
    {
        //arrange

        _auctionRepository.Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(value: null);

        //act
        var result = await _controller.GetAuctionById(Guid.NewGuid());
        //assert

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateAuction_WithValidCreateAuctionDto_ReturnsCreatedAtActionResult()
    {
        //arrange
        var auction = _fixture.Create<CreateAuctionDto>();
        _auctionRepository.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
        _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);
        //act
        var result = await _controller.CreateAuction(auction);
        var createdReult = result.Result as CreatedAtActionResult;
        //assert
        Assert.NotNull(createdReult);
        Assert.Equal("GetAuctionById", createdReult.ActionName);
        Assert.IsType<AuctionDto>(createdReult.Value);
    }

    [Fact]
    public async Task CreateAuction_FailedSave_Returns400BadRequest()
    {
        //arrange
        var auction = _fixture.Create<CreateAuctionDto>();
        _auctionRepository.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
        _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(false);
        //act
        var result = await _controller.CreateAuction(auction);
        //assert

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateAuction_WithUpdateAuctionDto_ReturnsOkResponse()
    {
        //arrange
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
        auction.Seller = "test";
        var upateDto = _fixture.Create<UpdateAuctionDto>();
        _auctionRepository.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
        _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

        //act
        var result = await _controller.UpdateAuction(auction.Id, upateDto);
        //assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UpdateAuction_WithInvalidUser_Returns403Forbid()
    {
        //arrange
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Seller = "not-test";
        var upateDto = _fixture.Create<UpdateAuctionDto>();
        _auctionRepository.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);

        //act
        var result = await _controller.UpdateAuction(auction.Id, upateDto);
        //assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task UpdateAuction_WithInvalidGuid_ReturnsNotFound()
    {
        //arrange
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        var upateDto = _fixture.Create<UpdateAuctionDto>();
        _auctionRepository.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(value: null);

        //act
        var result = await _controller.UpdateAuction(auction.Id, upateDto);
        //assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteAuction_WithValidUser_ReturnsOkResponse()
    {
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Seller = "test";
        _auctionRepository.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
        //_auctionRepository.Setup(repo => repo.RemoveAuction(auction));
        _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);
        //act
        var result = await _controller.DeleteAuction(auction.Id);
        //assert
        Assert.IsType<OkResult>(result);

    }

    [Fact]
    public async Task DeleteAuction_WithInvalidGuid_Returns404Response()
    {
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        _auctionRepository.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(value: null);
        //act
        var result = await _controller.DeleteAuction(auction.Id);
        //assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteAuction_WithInvalidUser_Returns403Response()
    {
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Seller = "not-test";
        _auctionRepository.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);

        //act
        var result = await _controller.DeleteAuction(auction.Id);
        //assert
        Assert.IsType<ForbidResult>(result);

    }

}
