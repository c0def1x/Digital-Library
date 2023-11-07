using Digital_Library.Data;
using Digital_Library.Domain.Entities;
using Digital_Library.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Digital_Library.Infrastructure
{
    public class EFRepository<T> :IRepository<T> where T : Entity
    {
        private readonly DigitalLibraryContext _context;

        public EFRepository (DigitalLibraryContext context)
        {
            _context = context;
        }
        public async Task<T> AddAsync (T entity)
        {
            _context.Entry(entity).State = EntityState.Added;
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync (T entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
        }

        public async Task<T?> FindAsync (int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<List<T>> FindWhere (Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<List<T>> GetAllAsync ()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> UpdateAsync (T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
