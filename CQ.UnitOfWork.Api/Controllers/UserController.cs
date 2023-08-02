using CQ.UnitOfWork.Core;
using Microsoft.AspNetCore.Mvc;

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
    }

    public class User
    {
        public string Id { get; set; }
    }
}