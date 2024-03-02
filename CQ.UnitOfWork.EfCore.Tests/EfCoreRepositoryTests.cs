using CQ.UnitOfWork.EfCore.Abstractions;
using CQ.Utility;
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
                Id = Db.NewId(),
                Name = "some name"
            };

            var result = await this._efCoreRepository.CreateAsync(newUser).ConfigureAwait(false);

            result.Should().NotBeNull();
            result.Id.Should().Be(newUser.Id);
            result.Name.Should().Be("some name");
        }

        [TestMethod]
        public async Task GetPagedAsync_WhenPageAndPageSize_ShouldReturnPagination()
        {
            var newUser = new TestUser
            {
                Id = Db.NewId(),
                Name = "some name"
            };

            var newUser2 = new TestUser
            {
                Id = Db.NewId(),
                Name = "some name"
            };

            await this._testContext.AddRangeAsync(newUser, newUser2).ConfigureAwait(false);
            await this._testContext.SaveChangesAsync().ConfigureAwait(false);

            var result = await this._efCoreRepository.GetPagedAsync(page: 1, pageSize: 1).ConfigureAwait(false);

            result.Should().NotBeNull();
            result.TotalItems.Should().Be(2);
            result.TotalPages.Should().Be(2);

            result.Items.Should().Contain(newUser);
            result.Items.Should().Contain(newUser2);
        }


        [TestMethod]
        public async Task GetPagedAsync_WhenWithDecimalPageCount_ShouldReturnTotalPagesBiggest()
        {
            var newUser = new TestUser
            {
                Id = Db.NewId(),
                Name = "some name"
            };

            var newUser2 = new TestUser
            {
                Id = Db.NewId(),
                Name = "some name"
            };

            var newUser3 = new TestUser
            {
                Id = Db.NewId(),
                Name = "some name"
            };

            await this._testContext.AddRangeAsync(newUser, newUser2, newUser3).ConfigureAwait(false);
            await this._testContext.SaveChangesAsync().ConfigureAwait(false);

            var result = await this._efCoreRepository.GetPagedAsync(page: 1, pageSize: 2).ConfigureAwait(false);

            result.Should().NotBeNull();
            result.TotalPages.Should().Be(2);
        }

        [TestMethod]
        public async Task UpdateByIdAsync_WhenDataExists_ShouldUpdateOnDataBase()
        {
            var user = new TestUser
            {
                Id = Db.NewId(),
                Name = "some name"
            };
            await this._testContext.AddAsync(user).ConfigureAwait(false);
            await this._testContext.SaveChangesAsync().ConfigureAwait(false);

            await this._efCoreRepository.UpdateByIdAsync(user.Id, new { Name = "updated" }).ConfigureAwait(false);
            var userUpdated = await this._testContext.Set<TestUser>().FirstAsync(u => u.Id == user.Id).ConfigureAwait(false);

            userUpdated.Should().NotBeNull();
            userUpdated.Name.Should().Be("updated");
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

        public TestContext(DbContextOptions options) : base(options)
        {
        }
    }
}