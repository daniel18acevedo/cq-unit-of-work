using CQ.UnitOfWork.Core.EfCore;
using CQ.UnitOfWork.Core.Mongo;
using CQ.UnitOfWork.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork.Entities;

namespace CQ.UnitOfWork.Core
{
    public interface IUnitOfWork
    {
        IRepository<TEntity> GetDefaultRepository<TEntity>() where TEntity : class;

        IEfCoreRepository<TEntity> GetEfCoreRepository<TEntity>() where TEntity : class;

        IMongoRepository<TEntity> GetMongoRepository<TEntity>(string? collectionName = null) where TEntity : class;
    }
}
