using AuctionService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTests;

public static class ServiceCollectionExtensions
{
    public static void RemoveDbContext<T>(this IServiceCollection services)
    {
        var descriptotr = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AuctionDbContexct>));
        if (descriptotr != null)
        {
            services.Remove(descriptotr);

        }
    }

    public static void EnsureCreated<T>(this IServiceCollection services)
    {
        var sp = services.BuildServiceProvider();

        using var scope = sp.CreateScope();
        var scopedSevices = scope.ServiceProvider;
        var db = scopedSevices.GetRequiredService<AuctionDbContexct>();
        db.Database.Migrate();
        DbHelper.InitDbForTests(db);
    }
}
