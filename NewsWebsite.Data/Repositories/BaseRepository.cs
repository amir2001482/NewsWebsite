using Microsoft.EntityFrameworkCore;
using NewsWebsite.Common;
using NewsWebsite.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NewsWebsite.Data.Repositories
{
    public class BaseRepository<TEntity,TContext> : IBaseRepository<TEntity> where TEntity : class where TContext : DbContext
    {
        protected TContext _Context { get; set; }
        private DbSet<TEntity> dbSet;
        public BaseRepository(TContext Context)
        {
            _Context = Context;
            _Context.CheckArgumentIsNull(nameof(_Context));
            dbSet = _Context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> FindAllAsync()
        {
            return await dbSet.AsNoTracking().ToListAsync();
        }

        public IEnumerable<TEntity> FindAll()
        {
            return dbSet.AsNoTracking().ToList();
        }

        public async Task<TEntity> FindByIdAsync(Object id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity,bool>> filter=null,Func<IQueryable<TEntity>,IOrderedQueryable<TEntity>> orderBy=null,params Expression<Func<TEntity,object>>[] includes)
        {
            IQueryable<TEntity> query = _Context.Set<TEntity>();
            foreach(var include in includes)
            {
                query = query.Include(include);
            }

            if(filter!=null)
            {
                query = query.Where(filter);
            }

            if(orderBy!=null)
            {
                query = orderBy(query);
            }

            return await query.ToListAsync();
        }

        public async Task CreateAsync(TEntity entity) => await dbSet.AddAsync(entity);

        public void Update(TEntity entity) => dbSet.Update(entity);

        public void Delete(TEntity entity) => dbSet.Remove(entity);

        public async Task CreateRangeAsync(IEnumerable<TEntity> entities) => await dbSet.AddRangeAsync(entities);

        public void UpdateRange(IEnumerable<TEntity> entities) => dbSet.UpdateRange(entities);

        public void DeleteRange(IEnumerable<TEntity> entities) => dbSet.RemoveRange(entities);

        public async Task<List<TEntity>> GetPaginateResultAsync(int currentPage,int pageSize=1)
        {
            var entities = await FindAllAsync();
            return entities.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        }

        public int CountEntities()
        {
            return dbSet.Count();
        }
    }
}
