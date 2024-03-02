using CQ.UnitOfWork.EfCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CQ.UnitOfWork.Api.EFCore.DataAccess
{

    public class ConcreteContext : EfCoreContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Book> Books { get; set; }

        public ConcreteContext(DbContextOptions<ConcreteContext> options) : base(options) { }
    }

    public class OtherConcreteContext : EfCoreContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Book> Books { get; set; }

        public OtherConcreteContext(DbContextOptions<OtherConcreteContext> options) : base(options) { }
    }


    public class User
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public User()
        {
            this.Id = Guid.NewGuid().ToString().Replace("-", "");
        }
    }

    public class MiniUser
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }

    public class Book
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Book()
        {
            this.Id = Guid.NewGuid().ToString().Replace("-", "");
        }
    }

    public class Other
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Other()
        {
            this.Id = Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}