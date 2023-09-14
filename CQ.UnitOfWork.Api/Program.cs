using CQ.UnitOfWork;
using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.Api.Controllers;
using CQ.UnitOfWork.EfCore;
using CQ.UnitOfWork.MongoDriver;
using dotenv.net;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
DotEnv.Load();

builder.Services.AddControllers();


builder.Services.AddUnitOfWork();




var efCoreConnectionString = Environment.GetEnvironmentVariable($"efcore-connection-string");
// "Filename=:memory:"
builder.Services.AddEfCoreContext<ConcreteContext>(new EfCoreConfig
{
    DatabaseConnection = new DatabaseConfig
    {
        ConnectionString = efCoreConnectionString,
        DatabaseName = "UnitOfWork"
    },
    Engine = EfCoreDataBaseEngine.SQL
});

builder.Services.AddEfCoreRepository<User>();









var mongoConnectionString = Environment.GetEnvironmentVariable($"mongo-connection-string");
builder.Services.AddMongoContext(
        new MongoConfig
        {
            DatabaseConnection = new DatabaseConfig
            {
                ConnectionString = mongoConnectionString,
                DatabaseName = "UnitOfWork"
            }
        });

builder.Services.AddMongoRepository<UserMongo>("Users");


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public class ConcreteContext : EfCoreContext
{
    public DbSet<User> Users { get; set; }
}