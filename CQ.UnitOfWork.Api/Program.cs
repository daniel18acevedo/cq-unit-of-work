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


builder.Services.AddUnitOfWork(LifeCycles.TRANSIENT);

var efCoreConnectionString = Environment.GetEnvironmentVariable($"efcore-connection-string");

builder.Services.AddEfCoreOrm(LifeCycles.SINGLETON, new ConcreteContext(efCoreConnectionString));

builder.Services.AddEfCoreRepository<User>(LifeCycles.SINGLETON);

var mongoConnectionString = Environment.GetEnvironmentVariable($"mongo-connection-string");
builder.Services.AddMongoDriverOrm(
        LifeCycles.SINGLETON,
        new MongoConfig
        {
            DatabaseConnection = new DatabaseConfig
            {
                ConnectionString = mongoConnectionString,
                DatabaseName = "UnitOfWork"
            }
        });

builder.Services.AddMongoRepository<UserMongo>(LifeCycles.SINGLETON, "Users");


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public class ConcreteContext : EfCoreContext
{
    public DbSet<User> Users { get; set; }

    public ConcreteContext(string connectionString) : base(new EfCoreConfig
    {
        EnabledDefaultQueryLogger = true,
        DatabaseConnection = new DatabaseConfig
        {
            ConnectionString = connectionString,
            DatabaseName = "UnitOfWork",
        }
    })
    { }
}