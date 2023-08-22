using CQ.UnitOfWork.Entities.DataAccessConfig;
using CQ.UnitOfWork.Entities;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                EnabledDefaultQueryLogger = true,
            });
        }
    }
}
