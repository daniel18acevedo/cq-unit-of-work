using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Abstractions
{
    public interface IUnitRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        void SetContext(IDatabaseContext context);

        #region Create entity
        /// <summary>
        /// Saves the entity pass as parameter into the data store with the ORM configured
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task CreateWithoutCommitAsync(TEntity entity);

        void CreateWithoutCommit(TEntity entity);
        #endregion
    }
}