using CQ.UnitOfWork.Entities;
using CQ.UnitOfWork.Entities.DataAccessConfig;
using CQ.UnitOfWork.Init;
using dotenv.net;
using UnitOfWork.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
DotEnv.Load();

builder.Services.AddControllers();

var mongoConnectionString = Environment.GetEnvironmentVariable($"mongo-db:connection-string");

builder.Services.AddUnitOfWorkWithMongo(LifeCycles.TRANSIENT, new MongoConfig
{
    EnabledDefaultQueryLogger = true,
    LifeCycle = LifeCycles.TRANSIENT,
    DataBaseConnection = new DataBaseConnection
    {
        ConnectionString = mongoConnectionString,
        DatabaseName = "UnitOfWork",
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
