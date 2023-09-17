using AuctionService.Data;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using WebMotions.Fake.Authentication.JwtBearer;


namespace AuctionService.IntegrationTests;

public class CustomWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime

{

    private readonly PostgreSqlContainer _postgresSqlContainer = new PostgreSqlBuilder().Build();
    public async Task InitializeAsync()
    {

        await _postgresSqlContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveDbContext<AuctionDbContexct>();
            services.AddDbContext<AuctionDbContexct>(options =>
            {
                options.UseNpgsql(_postgresSqlContainer.GetConnectionString());
            });

            services.AddMassTransitTestHarness();
            services.EnsureCreated<AuctionDbContexct>();
            services.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme)
            .AddFakeJwtBearer(opt =>
            {
                opt.BearerValueType = FakeJwtBearerBearerValueType.Jwt;
            });

        });

    }

    Task IAsyncLifetime.DisposeAsync() => _postgresSqlContainer.DisposeAsync().AsTask();

}
