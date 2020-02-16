using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.Models.Base;

namespace RepositoryLayer.Base
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        public T GetById(long id);

        public Task<T> GetByIdAsync(long id);

        public List<T> Get();

        public Task<List<T>> GetAsync();

        public T Create(T entity);

        public Task<T> CreateAsync(T entity);

        public void Delete(T entity);

        public Task DeleteAsync(T entity);

        public T Update(T entity);

        public Task<T> UpdateAsync(T entity);

        public List<T> UpdateRange(List<T> entities);

        public Task<List<T>> UpdateRangeAsync(List<T> entities);
    }

    public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
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

        public T GetById(long id)
        {
            return Entities.FirstOrDefault(e => e.Id == id);
        }

        public async Task<T> GetByIdAsync(long id)
        {
            return await Entities.FirstOrDefaultAsync(e => e.Id == id);
        }

        public List<T> Get()
        {
            return Entities.ToList();
        }

        public async Task<List<T>> GetAsync()
        {
            return await Entities.ToListAsync();
        }

        public T Create(T entity)
        {
            NullCheck(entity);
            _entitySet.Add(entity);
            _context.SaveChanges();

            return entity;
        }

        public async Task<T> CreateAsync(T entity)
        {
            NullCheck(entity);
            await _entitySet.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public void Delete(T entity)
        {
            NullCheck(entity);
            _entitySet.Remove(entity);
            _context.SaveChanges();
        }

        public async Task DeleteAsync(T entity)
        {
            NullCheck(entity);
            _entitySet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public T Update(T entity)
        {
            NullCheck(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            NullCheck(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public List<T> UpdateRange(List<T> entities)
        {
            NullCheck(entities);
            _context.Entry(entities).State = EntityState.Modified;
            _context.SaveChanges();
            return entities;
        }

        public async Task<List<T>> UpdateRangeAsync(List<T> entities)
        {
            NullCheck(entities);
            _context.Entry(entities).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entities;
        }

        private void NullCheck(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
        }

        private void NullCheck(List<T> entities)
        {
            if (entities == null || !entities.Any()) throw new ArgumentNullException(nameof(entities));
        }
    }
}