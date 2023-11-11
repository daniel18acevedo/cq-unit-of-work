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
        private readonly IMongoDriverRepository<UserMongo> _userUnitOfWorkMongoRepository;
        private readonly IRepository<UserMongo> _userUnitOfWorkGenericRepository;
        private readonly IRepository<UserMongo> _userGenericRepository;
        private readonly IRepository<OtherUserMongo> _otherUserGenericRepository;

        public UserMongoController(IUnitOfWork unitOfWork, IRepository<UserMongo> userGenericRepository, IRepository<OtherUserMongo> otherUserGenericRepository)
        {
            this._userUnitOfWorkMongoRepository = unitOfWork.GetRepository<IMongoDriverRepository<UserMongo>>();
            this._userUnitOfWorkGenericRepository = unitOfWork.GetEntityRepository<UserMongo>();
            this._userGenericRepository = userGenericRepository;
            this._otherUserGenericRepository = otherUserGenericRepository;
        }

        [HttpGet("all")]
        public async Task<IActionResult> Get()
        {
            var users = await this._userUnitOfWorkGenericRepository.GetAllAsync().ConfigureAwait(false);

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync([FromRoute] string id)
        {
            var user = await _userUnitOfWorkMongoRepository.GetByPropAsync(id).ConfigureAwait(false);

            return Ok(user);
        }

        [HttpGet("{id}/custom-exception")]
        public async Task<IActionResult> GetCustomExceptionAsync([FromRoute] string id)
        {
            try
            {
                var user = await this._userUnitOfWorkGenericRepository.GetAsync(u => u.Id == "no", new InvalidOperationException()).ConfigureAwait(false);

                return Ok(user);
            }
            catch (Exception ex) { return BadRequest(new { exception = ex.Message, innerException = ex.InnerException.Message }); }
        }

        [HttpGet("all/mini")]
        public async Task<IActionResult> GetMiniAsync()
        {
            var user = await _userUnitOfWorkMongoRepository.GetAllAsync<MiniUserMongo>().ConfigureAwait(false);

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync()
        {
            var user = new UserMongo
            {
                Name = "test",
            };

            var userCreated = await _userUnitOfWorkMongoRepository.CreateAsync(user).ConfigureAwait(false);

            return Ok(userCreated);
        }
    }
}