using CQ.UnitOfWork.Entities;
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
            var ormToUse = orm ?? this._services.GetService<Orms>();

            var genericRepository = this.BuildGenericRepository<TEntity>(ormToUse);

            return genericRepository;
        }

        private IRepository<TEntity> BuildGenericRepository<TEntity>(Orms orm) where TEntity : class
        {
            switch (orm)
            {
                case Orms.MONGO_DB:
                    return this.BuildMongoGenericRepository<TEntity>();
                default:
                    throw new ArgumentException("Orm type not supported");
            }
        }

        private IRepository<TEntity> BuildMongoGenericRepository<TEntity>() where TEntity : class
        {
            var mongoDatabase = this._services.GetService<IMongoDatabase>();

            if (mongoDatabase is null)
            {
                throw new ArgumentNullException("database");
            }

            var genericRepository = new MongoRepository<TEntity>(mongoDatabase);

            return genericRepository;
        }
    }
}
