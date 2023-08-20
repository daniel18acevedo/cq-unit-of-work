using CQ.UnitOfWork.Entities;
using CQ.UnitOfWork.Entities.Context;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
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

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            var entityRepository = this._services.GetService<IRepository<TEntity>>();

            if (entityRepository is not null)
            {
                return entityRepository;
            }

            var genericRepository = this.GetGenericRepository<TEntity>();

            return genericRepository;
        }

        public IRepository<TEntity> GetGenericRepository<TEntity>(Orms? orm = null) where TEntity : class
        {
            if(orm is null)
            {
                var defaultContext = this._services.GetService<DatabaseContext>();

                if(defaultContext is null)
                {
                    throw new ArgumentException("Default orm not setted");
                }

                orm = defaultContext.Orm;
            }

            var genericRepository = this.BuildGenericRepository<TEntity>(orm.Value);

            return genericRepository;
        }

        private IRepository<TEntity> BuildGenericRepository<TEntity>(Orms orm) where TEntity : class
        {
            switch (orm)
            {
                case Orms.MONGO_DB:
                    return this.BuildMongoGenericRepository<TEntity>();
                case Orms.EF_CORE:
                    return this.BuildEfCoreGenericRepository<TEntity>();
                default:
                    throw new ArgumentException("Orm type not supported");
            }
        }

        private IRepository<TEntity> BuildMongoGenericRepository<TEntity>() where TEntity : class
        {
            var mongoContext = this._services.GetService<MongoContext>();

            if (mongoContext is null)
            {
                throw new ArgumentNullException("mongoContext");
            }

            var genericRepository = new MongoRepository<TEntity>(mongoContext);

            return genericRepository;
        }

        private IRepository<TEntity> BuildEfCoreGenericRepository<TEntity>() where TEntity : class
        {
            var efCoreContext = this._services.GetService<EfCoreContext>();

            if(efCoreContext is null)
            {
                throw new ArgumentNullException("efCoreContext");
            }

            var genericEfCoreRepository = new EfCoreRepository<TEntity>(efCoreContext);

            return genericEfCoreRepository;
        }
    }
}
