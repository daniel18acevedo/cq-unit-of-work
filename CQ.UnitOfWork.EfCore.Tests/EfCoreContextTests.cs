using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.EfCore.Tests
{
    [TestClass]
    public class EfCoreContextTests
    {
        private readonly DbConnection _connection;
        private readonly EfCoreContext _testContext;

        public EfCoreContextTests()
        {
            this._connection = new SqliteConnection("Filename=:memory:");
            var contextOptions = new DbContextOptionsBuilder<TestContext>().UseSqlite(this._connection).Options;
            this._testContext = new TestContext(contextOptions);
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
        public void GetTableName_WhenSetNameDifferentThanType_ShouldReturnPropertyName()
        {
            var name = this._testContext.GetTableName<TestUser>();

            name.Should().NotBeNull();
            name.Should().Be("Users");
        }

        [TestMethod]
        public void GetDatabaseInfo_WhenContextSetted_ShouldReturnBasicInfo()
        {
            var databaseInfo = this._testContext.GetDatabaseInfo();

            databaseInfo.Provider.Should().Contain("Sqlite");
        }
    }
}
