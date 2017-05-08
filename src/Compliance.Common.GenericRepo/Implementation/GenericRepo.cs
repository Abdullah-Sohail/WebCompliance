using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;

namespace Compliance.Common.GenericRepo.Implementation
{
    public class GenericRepo<TEntity, TContext> : IGenericRepo<TEntity, TContext>
        where TEntity : class, ICanGenericRepo
    {

        private readonly DbContext _context;

        protected DbContext context { get { return _context; } }

        public GenericRepo(IMakeDbContext<TContext> context)
        {
            this._context = context.GetContext();
        }

        public TEntity GetById(Guid id)
        {
            return this._context.Set<TEntity>().Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return this._context.Set<TEntity>();
        }

        public IEnumerable<TEntity> Query(Expression<Func<TEntity, bool>> filter, string[] includes = null)
        {

            if (includes == null) return this._context.Set<TEntity>().Where(filter);

            switch (includes.GetUpperBound(0))
            {
                case 0:
                    return this._context.Set<TEntity>()
                        .Include(includes[0])
                        .Where(filter);
                case 1:
                    return this._context.Set<TEntity>()
                        .Include(includes[0])
                        .Include(includes[1])
                        .Where(filter);
                case 2:
                    return this._context.Set<TEntity>()
                        .Include(includes[0])
                        .Include(includes[1])
                        .Include(includes[2])
                        .Where(filter);
                case 3:
                    return this._context.Set<TEntity>()
                        .Include(includes[0])
                        .Include(includes[1])
                        .Include(includes[2])
                        .Include(includes[3])
                        .Where(filter);
                case 4:
                    return this._context.Set<TEntity>()
                        .Include(includes[0])
                        .Include(includes[1])
                        .Include(includes[2])
                        .Include(includes[3])
                        .Include(includes[4])
                        .Where(filter);
                case 5:
                    return this._context.Set<TEntity>()
                        .Include(includes[0])
                        .Include(includes[1])
                        .Include(includes[2])
                        .Include(includes[3])
                        .Include(includes[4])
                        .Include(includes[5])
                        .Where(filter);
                case 6:
                    return this._context.Set<TEntity>()
                        .Include(includes[0])
                        .Include(includes[1])
                        .Include(includes[2])
                        .Include(includes[3])
                        .Include(includes[4])
                        .Include(includes[5])
                        .Include(includes[6])
                        .Where(filter);
                case 7:
                    return this._context.Set<TEntity>()
                        .Include(includes[0])
                        .Include(includes[1])
                        .Include(includes[2])
                        .Include(includes[3])
                        .Include(includes[4])
                        .Include(includes[5])
                        .Include(includes[6])
                        .Include(includes[7])
                        .Where(filter);
                case 8:
                    return this._context.Set<TEntity>()
                        .Include(includes[0])
                        .Include(includes[1])
                        .Include(includes[2])
                        .Include(includes[3])
                        .Include(includes[4])
                        .Include(includes[5])
                        .Include(includes[6])
                        .Include(includes[7])
                        .Include(includes[8])
                        .Where(filter);
                case 9:
                    return this._context.Set<TEntity>()
                        .Include(includes[0])
                        .Include(includes[1])
                        .Include(includes[2])
                        .Include(includes[3])
                        .Include(includes[4])
                        .Include(includes[5])
                        .Include(includes[6])
                        .Include(includes[7])
                        .Include(includes[8])
                        .Include(includes[9])
                        .Where(filter);
                default:
                    throw new Exception("You can only include up to 10 additional entities");
            }
        }

        public void Add(TEntity entity)
        {
            this._context.Set<TEntity>().Add(entity);
        }

        public void Remove(TEntity entity)
        {
            this._context.Set<TEntity>().Remove(entity);
        }

        public void Update(TEntity entity)
        {
            this._context.Entry(entity).State = EntityState.Modified;
        }

        public void Load(TEntity entity, string propToLoad)
        {
            _context.Entry(entity)
                .Collection(propToLoad)
                .Load();
        }

        public void Save()
        {
            this._context.SaveChanges();
        }
    }
}
