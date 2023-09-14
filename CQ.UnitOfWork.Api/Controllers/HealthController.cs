using CQ.UnitOfWork.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CQ.UnitOfWork.Api.Controllers
{
    [ApiController]
    [Route("/", Name = "Ping")]
    [Route("health", Name = "HealthCheck")]
    public class HealthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDatabaseContext? _dataBaseContext;

        public HealthController(IUnitOfWork unitOfWork, IDatabaseContext databaseContext)
        {
            this._unitOfWork = unitOfWork;
            this._dataBaseContext= databaseContext;
        }

        [HttpGet]
        public object Get()
        {

            return new
            {
                Alive = true,
                DatabaseConnection = this._dataBaseContext == null ? false : this._dataBaseContext.Ping()
            };
        }
    }
}
