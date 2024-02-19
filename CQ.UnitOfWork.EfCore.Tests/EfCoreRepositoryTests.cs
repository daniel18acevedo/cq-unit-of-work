using CQ.UnitOfWork.EfCore.Abstractions;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace CQ.UnitOfWork.EfCore.Tests
{
    [TestClass]
    public class EfCoreRepositoryTests
    {
        private readonly DbConnection _connection;
        private readonly TestContext _testContext;
        private readonly IEfCoreRepository<TestUser> _efCoreRepository;

        public EfCoreRepositoryTests()
        {
            this._connection = new SqliteConnection("Filename=:memory:");
            var contextOptions = new DbContextOptionsBuilder<TestContext>().UseSqlite(this._connection).Options;
            this._testContext = new TestContext(contextOptions);
            this._efCoreRepository = new EfCoreRepository<TestUser>(this._testContext);
        }

        [TestInitialize]
        public void SetUp()
        {
            this._connection.Open();
            this._testContext.Database.EnsureCreated();
        }

        [TestCleanup]
        public void CleanUp()
        {
            this._testContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task CreateAsync_WhenNewUser_ShouldSaveNewUser()
        {
            var newUser = new TestUser
            {
                Id = Guid.NewGuid().ToString(),
                Name = "some name"
            };

            var result = await this._efCoreRepository.CreateAsync(newUser).ConfigureAwait(false);

            result.Should().NotBeNull();
            result.Id.Should().Be(newUser.Id);
            result.Name.Should().Be("some name");
        }
    }

    internal sealed record class TestUser
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }

    internal sealed class TestContext : EfCoreContext
    {
        public DbSet<TestUser> Users { get; set; }

        public TestContext(EfCoreConfig config)
            : base(config)
        {
        }

        public TestContext(DbContextOptions options) : base(options) 
        { 
        }
    }
}