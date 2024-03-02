using CQ.ServiceExtension;
using CQ.UnitOfWork;
using CQ.UnitOfWork.Api.EFCore.DataAccess;
using CQ.UnitOfWork.Api.MongoDriver.DataAccess;
using CQ.UnitOfWork.EfCore;
using CQ.UnitOfWork.MongoDriver;
using dotenv.net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
DotEnv.Load();

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});


builder.Services.AddUnitOfWork();




var efCoreConnectionString = Environment.GetEnvironmentVariable($"efcore-connection-string");
// "Filename=:memory:"
builder.Services.AddEfCoreContext<ConcreteContext>(new EfCoreConfig(
    new DatabaseConfig(efCoreConnectionString, "UnitOfWork"),
    EfCoreDataBaseEngine.SQL,
    useDefaultQueryLogger: true));

builder.Services
    .AddEfCoreRepository<User>(LifeTime.Transient)
    .AddEfCoreRepository<Book>(LifeTime.Transient);


var otherEfCoreConnectionString = Environment.GetEnvironmentVariable($"other-efcore-connection-string");

builder.Services.AddEfCoreContext<OtherConcreteContext>(new EfCoreConfig(
    new DatabaseConfig(otherEfCoreConnectionString, "UnitOfWorkOther"),
    EfCoreDataBaseEngine.SQL,
    useDefaultQueryLogger: true,
    @default: false));

builder.Services
    .AddEfCoreRepository<Other>("UnitOfWorkOther", LifeTime.Transient);




var mongoConnectionString = Environment.GetEnvironmentVariable($"mongo-connection-string");
builder.Services.AddMongoContext<UnitOfWorkMongoContext>(
        new MongoConfig(
            new DatabaseConfig(mongoConnectionString, "UnitOfWork"),
        true,
        true));

builder.Services.AddMongoContext(
        new MongoConfig(new DatabaseConfig(mongoConnectionString, "OtherDatabase"),
        useDefaultQueryLogger: true));

builder.Services
    .AddMongoRepository<UserMongo>()
    .AddMongoRepository<OtherUserMongo>("OtherDatabase");


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
