using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using MyProject.Models;

namespace MyProject.Repository
{
    internal class GenericRepository<T> : IRepository<T>
    where T : class
    {
        private classRoomWebSiteDBContext entities;

        private IDbSet<T> _dbSet;

        public GenericRepository(classRoomWebSiteDBContext _entities)
        {
            this.entities = _entities;
            this._dbSet = this.entities.Set<T>();
        }

        public void Add(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("�L�k�s�W���");
            }
            this.entities.Entry(entity).State = EntityState.Added;
            this.entities.Set<T>().Add(entity);
        }

        public IQueryable<T> All()
        {
            return this.entities.Set<T>().AsQueryable<T>();
        }

        public void Attach(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("�L�k�ק���");
            }
            this.entities.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("�L�k�R�����");
            }
            this.entities.Entry(entity).State = EntityState.Deleted;
            this.entities.Set<T>().Remove(entity);

        }

        public T Get(Func<T, bool> predicate)
        {
            return this._dbSet.First<T>(predicate);
        }

        public IEnumerable<T> GetAll(Func<T, bool> predicate = null)
        {
            if (predicate == null)
            {
                return this._dbSet.AsEnumerable<T>();
            }
            return this._dbSet.Where<T>(predicate);
        }
    }
}
 
