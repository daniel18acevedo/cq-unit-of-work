using CQ.UnitOfWork.Core;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;

namespace CQ.UnitOfWork.Api.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IRepository<User> _repository;

        public UserController(IUnitOfWork unitOfWork)
        {
            this._repository = unitOfWork.GetRepository<User>();
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync([FromRoute]string id)
        {
            var user = await _repository.GetByPropAsync(id).ConfigureAwait(false);

            return Ok(user);
        }

        [HttpGet("create")]
        public async Task<IActionResult> CreateAsync()
        {
            var user = new User
            {
                Name = "test",
                Book = new Book
                {
                    Name="Harry potter"
                }
            };

            var userCreated = await _repository.CreateAsync(user).ConfigureAwait(false);

            return Ok(userCreated);
        }
    }

    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

        public Book Book { get; set; }

        public User()
        {
            Id = Guid.NewGuid().ToString().Replace("-", "");
        }
    }

    [BsonIgnoreExtraElements]
    public class Book
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

        public Book()
        {
            Id = Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}