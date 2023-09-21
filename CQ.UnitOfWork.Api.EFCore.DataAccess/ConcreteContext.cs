using CQ.UnitOfWork.EfCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CQ.UnitOfWork.Api.EFCore.DataAccess
{

    public class ConcreteContext : EfCoreContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Book> Books { get; set; }

        public ConcreteContext() { }

        public ConcreteContext(EfCoreConfig config) : base(config) { }

        public ConcreteContext(DbContextOptions options) : base(options) { }
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

    public class Book
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Book()
        {
            this.Id = Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}