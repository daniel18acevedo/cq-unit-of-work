using CQ.UnitOfWork.Entities;
using CQ.UnitOfWork.Init;
using dotenv.net;
using UnitOfWork.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
DotEnv.Load();

builder.Services.AddControllers();

var defaultMongoDb = Environment.GetEnvironmentVariable("mongo-db:default");
var mongoConnectionString = Environment.GetEnvironmentVariable($"mongo-db:{defaultMongoDb}-connection-string");
builder.Services.AddMongoDatabase(mongoConnectionString, "UnitOfWork");
builder.Services.AddUnitOfWork(Orms.MONGO_DB);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
