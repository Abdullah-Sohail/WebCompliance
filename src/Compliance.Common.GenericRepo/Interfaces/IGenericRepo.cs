using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Compliance.Common.GenericRepo.Interfaces
{
    public interface IGenericRepo<TEntity, TContext>
        where TEntity : class, ICanGenericRepo
    {
        TEntity GetById(Guid id);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Query(Expression<Func<TEntity, bool>> filter, string[] includes = null);
        void Add(TEntity entity);
        void Remove(TEntity entity);
        void Update(TEntity entity);
        void Load(TEntity entity, string propToLoad);
        void Save();
    }
}