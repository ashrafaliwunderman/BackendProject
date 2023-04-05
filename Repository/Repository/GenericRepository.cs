using Core.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected DbContext context;
        protected DbSet<TEntity> dbSet;

        public virtual async Task DeleteData(int id)
        {
            var data = dbSet.Find(id);
            dbSet.Remove(data);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = dbSet;

            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            return await query.ToListAsync<TEntity>();
        }

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            return query;
        }

        public virtual ValueTask<TEntity> GetByIdAsync(object id)
        {
            return dbSet.FindAsync(id);
        }

        public virtual async Task InsertData(TEntity entity)
        {
            var temp = dbSet.Add(entity);
            context.Entry(entity).State = EntityState.Added;
        }

        public virtual async Task UpdateData(TEntity entity)
        {
            dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        protected int SaveChanges()
        {
            return context.SaveChanges();
        }

        protected Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }

        public void SetContext(DbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }
    }
}
