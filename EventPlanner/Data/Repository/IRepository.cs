using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EventPlanner.Data.Repository
{
    public interface IRepository<T> where T : class, new()
    {
        IEnumerable<T> Get(Expression<Func<T, bool>> conditions, params Expression<Func<T, object>>[] includes);
        IEnumerable<T> Get(Expression<Func<T, bool>> conditions);
        IEnumerable<T> Get(params Expression<Func<T, object>>[] includes);

        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void AddOrUpdate(T entity);

    }
}
