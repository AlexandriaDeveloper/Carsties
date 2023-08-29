using MongoDB.Driver;
using MongoDB.Entities;
using Polly;
using Polly.Extensions.Http;
using SearchService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddHttpClient<AuctionServiceHttpClient>().AddPolicyHandler(GetPolicy());
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.



app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        await DbInitilizer.InitDb(app);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);

    }
});



app.Run();


static IAsyncPolicy<HttpResponseMessage> GetPolicy()
=> HttpPolicyExtensions
.HandleTransientHttpError()
.WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromSeconds(3));
