using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork.Entities;

namespace CQ.UnitOfWork.Core
{
    internal class UnitOfWorkService : IUnitOfWork
    {
        private readonly IServiceProvider _services;

        public UnitOfWorkService(IServiceProvider services)
        {
            this._services = services;
        }

        public IRepository<T> GetRepository<T>() where T : class 
        {
            var genericRepository = this._services.GetService<IRepository<T>>();

            if (genericRepository is null)
            {
                throw new ArgumentException($"Default repository not injected");
            }

            return genericRepository;
        }
    }
}
