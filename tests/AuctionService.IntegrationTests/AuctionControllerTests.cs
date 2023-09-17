
using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTests;
[Collection("Shared Collection")]
public class AuctionControllerTests : IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private readonly HttpClient _httpClient;
    private const string GT_ID = "afbee524-5972-4075-8800-7d1f9d7b0a0c";
    public AuctionControllerTests(CustomWebAppFactory factory)
    {
        this._factory = factory;
        _httpClient = _factory.CreateClient();
    }
    [Fact]
    public async Task GetAuctions_ShouldReturn3Auctions()
    {
        //arrange
        //act
        var response = await _httpClient.GetFromJsonAsync<List<AuctionDto>>("api/auctions/");
        //assert
        Assert.Equal(3, response.Count);
    }

    [Fact]
    public async Task GetAuctionById_WithValidIdShouldReturnAuction()
    {
        //arrange
        //act
        var response = await _httpClient.GetFromJsonAsync<AuctionDto>($"api/auctions/{GT_ID}");
        //assert
        Assert.Equal("GT", response.Model);
    }
    [Fact]
    public async Task GetAuctionById_WithInvalidIdShouldReturn404()
    {
        //arrange
        //act
        var response = await _httpClient.GetAsync($"api/auctions/{Guid.NewGuid()}");
        //assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    [Fact]
    public async Task GetAuctionById_WithInvalidIdShouldReturn400()
    {
        //arrange
        //act
        var response = await _httpClient.GetAsync($"api/auctions/not-guid");
        //assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateAuction_WithNoAuth_ShouldReturn401()
    {
        //arrange
        var auction = new AuctionDto() { Make = "test" };
        //act
        var response = await _httpClient.PostAsJsonAsync($"api/auctions", auction);
        //assert

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }


    [Fact]
    public async Task CreateAuction_WithAuth_ShouldReturn201()
    {
        //arrange
        var auction = GetAuctionForCreate();
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));
        //act
        var response = await _httpClient.PostAsJsonAsync($"api/auctions", auction);
        //assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdAuction = await response.Content.ReadFromJsonAsync<AuctionDto>();
        Assert.Equal("bob", createdAuction.Seller);
    }


    [Fact]
    public async Task CreateAuction_WithInvalidCreateAuctionDto_ShouldReturn400()
    {
        // arrange
        var auction = new AuctionDto() { Make = null };
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));

        // act
        var response = await _httpClient.PostAsJsonAsync($"api/auctions", auction);
        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuction_WithValidUpdateDtoAndUser_ShouldReturn200()
    {
        // arrange
        var auction = GetAuctionForCreate();
        auction.Make = "updated";
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));

        // act
        var response = await _httpClient.PutAsJsonAsync($"api/auctions/{GT_ID}", auction);
        // assert

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuction_WithValidUpdateDtoAndInvalidUser_ShouldReturn403()
    {
        // arrange 
        var auction = GetAuctionForCreate();
        auction.Make = "updated";
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("not-bob"));
        // act
        var response = await _httpClient.PutAsJsonAsync($"api/auctions/{GT_ID}", auction);
        // assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuctionDbContexct>();
        DbHelper.ReinitDbForTests(db);
        return Task.CompletedTask;
    }
    private static CreateAuctionDto GetAuctionForCreate()
    {
        return new CreateAuctionDto
        {
            Make = "test",
            Model = "test",
            ImageUrl = "test",
            Color = "test",
            Mileage = 10,
            Year = 10,
            ReservePrice = 10
        };
    }

}
