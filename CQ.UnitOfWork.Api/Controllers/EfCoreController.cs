using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.Api.EFCore.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace CQ.UnitOfWork.Api.Controllers
{
    [ApiController]
    [Route("efCore")]
    public class EfCoreController : ControllerBase
    {
        public EfCoreController(IRepository<User> otherRepository) { }

        [HttpGet]
        public void Get()
        {
        }
    }
}
