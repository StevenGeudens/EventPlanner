using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;

namespace EventPlanner.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class, new()
    {
        protected EventPlannerContext Context { get; }
        public Repository(EventPlannerContext context)
        {
            this.Context = context;
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> conditions, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = Context.Set<T>();
            if(includes != null)
            {
                foreach(var include in includes) 
                {
                    query= query.Include(include);
                }
            }
            if (conditions != null)
            {
                query = query.Where(conditions);
            }
            return query.ToList();
        }
        public IEnumerable<T> Get(Expression<Func<T, bool>> conditions)
        {
            return Get(conditions, null).ToList();
        }
        public IEnumerable<T> Get(params Expression<Func<T, object>>[] includes)
        {
            return Get(null, includes).ToList();
        }

        public void Add(T entity)
        {
            Context.Set<T>().Add(entity);
        }
        public void Delete(T entity)
        {
            Context.Entry(entity).State = EntityState.Deleted;
        }
        public void Update(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }
        public void AddOrUpdate(T entity)
        {
            // If PK = 0, then add
            // If PK > 0, then update
            Context.Set<T>().Update(entity);
        }
    }
}
