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

var connectionString = Environment.GetEnvironmentVariable($"connection-string");

builder.Services.AddUnitOfWorkWithEfCore(LifeCycles.TRANSIENT, new OrmServiceConfig<ConcreteContext>
{
    LifeCycle = LifeCycles.SINGLETON,
    Config = new ConcreteContext(connectionString)
});

builder.Services.AddEfCoreRepository<User>(LifeCycles.SINGLETON);

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
        DataBaseConnection = new DatabaseConfig
        {
            ConnectionString = connectionString,
            DatabaseName = "UnitOfWork",
        }
    })
    { }
}