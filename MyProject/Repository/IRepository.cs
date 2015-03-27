using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProject.Repository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Func<T, bool> predicate = null);
        IQueryable<T> All();
        T Get(Func<T, bool> predicate);
        void Add(T entity);
        void Attach(T entity);
        void Delete(T entity);
    }
}
 
