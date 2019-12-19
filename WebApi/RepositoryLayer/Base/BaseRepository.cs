
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.Models.Base;

namespace RepositoryLayer.Base
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        T GetById(string id);
        List<T> Get();
        T Create(T entity);
        void Delete(T entity);
        void Update(T entity);
    }

    public abstract class BaseRepository<T> : IBaseRepository<T> where T: BaseEntity
    {
        private readonly CrmContext _context;
        private readonly DbSet<T> _entitySet;
        public IQueryable<T> Entities;


        protected BaseRepository(CrmContext context)
        {
            _context = context;
            _entitySet = context.Set<T>();
            Entities = _entitySet;
        }

        public T GetById(string id) 
        {
            return Entities.FirstOrDefault(e => e.Id == id);
        }

        public List<T> Get() 
        {
            return Entities.ToList();
        }

        public T Create(T entity)
        {
            // TODO add null check
            _entitySet.Add(entity);
            _context.SaveChanges();

            return entity;
        }

        public void Delete(T entity)
        {
            // TODO add null check
            _entitySet.Remove(entity);
            _context.SaveChanges();
        }

        public void Update(T entity) 
        {
            // TODO add null check_
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}


        
