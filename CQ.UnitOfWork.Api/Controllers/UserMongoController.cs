using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.Api.MongoDriver.DataAccess;
using CQ.UnitOfWork.MongoDriver;
using CQ.UnitOfWork.MongoDriver.Abstractions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;

namespace CQ.UnitOfWork.Api.Controllers
{
    [ApiController]
    [Route("mongo/users")]
    public class UserMongoController : ControllerBase
    {
        private readonly IMongoDriverRepository<UserMongo> _repository;
        private readonly IRepository<UserMongo> _genericRepository;

        public UserMongoController(IUnitOfWork unitOfWork)
        {
            this._repository = unitOfWork.GetRepository<IMongoDriverRepository<UserMongo>>();
            this._genericRepository= unitOfWork.GetEntityRepository<UserMongo>();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await this._genericRepository.GetAllAsync().ConfigureAwait(false);

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
}