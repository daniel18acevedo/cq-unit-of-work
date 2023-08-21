using CQ.UnitOfWork.Core;
using CQ.UnitOfWork.Core.EfCore;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;

namespace CQ.UnitOfWork.Api.Controllers
{
    [ApiController]
    [Route("ef-core/users")]
    public class UserEfCoreController : ControllerBase
    {
        private readonly IEfCoreRepository<User> _repository;

        public UserEfCoreController(IUnitOfWork unitOfWork)
        {
            this._repository = unitOfWork.GetEfCoreRepository<User>();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await this._repository.GetAllAsync().ConfigureAwait(false);

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync([FromRoute] string id)
        {
            var user = await _repository.GetByPropAsync(id).ConfigureAwait(false);

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync()
        {
            var user = new User
            {
                Name = "test",
            };

            var userCreated = await _repository.CreateAsync(user).ConfigureAwait(false);

            return Ok(userCreated);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string id)
        {
            var user = await this._repository.GetByPropAsync(id).ConfigureAwait(false);
            user.Name = "new name";

            await this._repository.UpdateAsync(user).ConfigureAwait(false);

            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string id)
        {
            await this._repository.DeleteAsync(user => user.Id == id).ConfigureAwait(false);
            return Ok();
        }
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
}