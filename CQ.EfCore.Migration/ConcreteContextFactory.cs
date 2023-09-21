using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.Api.EFCore.DataAccess;
using CQ.UnitOfWork.EfCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CQ.EfCore.Migrations
{
    public class ConcreteContextFactory : IDesignTimeDbContextFactory<ConcreteContext>
    {
        public ConcreteContext CreateDbContext(string[] args)
        {
            return new ConcreteContext(new EfCoreConfig
            {
                DatabaseConnection = new DatabaseConfig
                {
                    ConnectionString = "Server=localhost;Database=UnitOfWork; Integrated Security=True;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True",
                    DatabaseName = "UnitOfWork"
                },
                UseDefaultQueryLogger = true,
                Engine = EfCoreDataBaseEngine.SQL,
            });
        }
    }
}
