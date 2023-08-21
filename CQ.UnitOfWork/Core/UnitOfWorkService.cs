using CQ.UnitOfWork.Core.EfCore;
using CQ.UnitOfWork.Core.Mongo;
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

        public IRepository<TEntity> GetDefaultRepository<TEntity>() where TEntity : class
        {
            var entityRepository = this._services.GetService<IRepository<TEntity>>();

            if (entityRepository is not null)
            {
                return entityRepository;
            }

            var genericRepository = this.GetDefaultRepository<TEntity>();

            return genericRepository;
        }

        public IEfCoreRepository<TEntity> GetEfCoreRepository<TEntity>() where TEntity : class
        {
            var efCoreEntityRepository = this._services.GetService<IEfCoreRepository<TEntity>>();

            if (efCoreEntityRepository is not null)
            {
                return efCoreEntityRepository;
            }

            var efCoreGenericRepository = this.BuildEfCoreGenericRepository<TEntity>();

            return efCoreGenericRepository;
        }

        public IMongoRepository<TEntity> GetMongoRepository<TEntity>(string? collectionName = null) where TEntity : class
        {
            var mongoEntityRepository = this._services.GetService<IMongoRepository<TEntity>>();

            if (mongoEntityRepository is not null)
            {
                return mongoEntityRepository;
            }

            var mongoGenericRepository = this.BuildMongoGenericRepository<TEntity>(collectionName);

            return mongoGenericRepository;
        }

        private IMongoRepository<TEntity> BuildMongoGenericRepository<TEntity>(string? collectionName = null) where TEntity : class
        {
            var mongoContext = this._services.GetService<MongoConnection>();

            if (mongoContext is null)
            {
                throw new ArgumentNullException("mongoContext");
            }

            var genericRepository = new MongoRepository<TEntity>(mongoContext, collectionName);

            return genericRepository;
        }

        private IEfCoreRepository<TEntity> BuildEfCoreGenericRepository<TEntity>() where TEntity : class
        {
            var efCoreContext = this._services.GetService<EfCoreConnection>();

            if (efCoreContext is null)
            {
                throw new ArgumentNullException("efCoreContext");
            }

            var genericEfCoreRepository = new EfCoreRepository<TEntity>(efCoreContext);

            return genericEfCoreRepository;
        }
    }
}
