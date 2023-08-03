using CQ.UnitOfWork.Core;
using CQ.UnitOfWork.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CQ.UnitOfWork.Api.Controllers
{
    [ApiController]
    [Route("/", Name = "Ping")]
    [Route("health", Name = "HealthCheck")]
    public class HealthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DataBaseContext? _dataBaseContext;

        public HealthController(IUnitOfWork unitOfWork, DataBaseContext databaseContext)
        {
            _unitOfWork = unitOfWork;
            _dataBaseContext = databaseContext;
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
