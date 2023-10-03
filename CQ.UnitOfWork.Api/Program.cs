using CQ.UnitOfWork;
using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.Api.EFCore.DataAccess;
using CQ.UnitOfWork.Api.MongoDriver.DataAccess;
using CQ.UnitOfWork.EfCore;
using CQ.UnitOfWork.MongoDriver;
using dotenv.net;

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
    Engine = EfCoreDataBaseEngine.SQL,
    UseDefaultQueryLogger= true
});

builder.Services.AddEfCoreRepository<User>(LifeTime.Transient);
builder.Services.AddEfCoreRepository<Book>(LifeTime.Transient);






var mongoConnectionString = Environment.GetEnvironmentVariable($"mongo-connection-string");
builder.Services.AddMongoContext(
        new MongoConfig
        {
            DatabaseConnection = new DatabaseConfig
            {
                ConnectionString = mongoConnectionString,
                DatabaseName = "UnitOfWork"
            },
            UseDefaultQueryLogger= true
        });

builder.Services.AddMongoRepository<UserMongo>("users");


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
