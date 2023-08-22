using CQ.UnitOfWork.Api.Controllers;
using CQ.UnitOfWork.Entities;
using CQ.UnitOfWork.Entities.Context;
using CQ.UnitOfWork.Entities.DataAccessConfig;
using CQ.UnitOfWork.Entities.ServiceConfig;
using CQ.UnitOfWork.Init;
using dotenv.net;
using Microsoft.EntityFrameworkCore;
using UnitOfWork.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
DotEnv.Load();

builder.Services.AddControllers();


builder.Services.AddUnitOfWork();






var efCoreConnectionString = Environment.GetEnvironmentVariable($"efcore-connection-string");
// "Filename=:memory:"
builder.Services.AddEfCoreOrm<ConcreteContext>(new EfCoreConfig
{

    DatabaseConnection = new DatabaseConfig
    {
        ConnectionString = efCoreConnectionString,
        DatabaseName = "UnitOfWork"
    },
});

builder.Services.AddEfCoreRepository<User>();









var mongoConnectionString = Environment.GetEnvironmentVariable($"mongo-connection-string");
builder.Services.AddMongoDriverOrm(
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

    public ConcreteContext(EfCoreConfig config) : base(config) { }
}