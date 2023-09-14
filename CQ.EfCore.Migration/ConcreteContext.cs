using CQ.UnitOfWork.EfCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.EfCore.Migrations
{
    public class ConcreteContext : EfCoreContext
    {
        public DbSet<User> Users { get; set; }

        public ConcreteContext(EfCoreConfig config) : base(config) { }
    }

    public class User
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
