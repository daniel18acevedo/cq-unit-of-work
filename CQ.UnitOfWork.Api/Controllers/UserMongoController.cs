using CQ.UnitOfWork.Core;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;

namespace CQ.UnitOfWork.Api.Controllers
{
    [ApiController]
    [Route("mongo/users")]
    public class UserMongoController : ControllerBase
    {
        private readonly IRepository<UserMongo> _repository;

        public UserMongoController(IUnitOfWork unitOfWork)
        {
            this._repository = unitOfWork.GetDefaultRepository<UserMongo>();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await this._repository.GetAllAsync().ConfigureAwait(false);

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync([FromRoute]string id)
        {
            var user = await _repository.GetByPropAsync(id).ConfigureAwait(false);

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync()
        {
            var user = new UserMongo
            {
                Name = "test",
            };

            var userCreated = await _repository.CreateAsync(user).ConfigureAwait(false);

            return Ok(userCreated);
        }
    }

    [BsonIgnoreExtraElements]
    public class UserMongo
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

        public BookMongo Book { get; set; }

        public UserMongo()
        {
            Id = Guid.NewGuid().ToString().Replace("-", "");
        }
    }

    [BsonIgnoreExtraElements]
    public class BookMongo
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

        public BookMongo()
        {
            Id = Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}