using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StaffBY.DAL.Context;
using StaffBY.DAL.Interfaces;

namespace StaffBY.DAL.Repositories
{
    /// <summary>
    /// Базовый репозиторий для работы с данными
    /// </summary>
    /// <typeparam name="T">Тип сущности</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly StaffBYDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(StaffBYDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Получить все записи
        /// </summary>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Получить запись по идентификатору
        /// </summary>
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Найти записи по условию
        /// </summary>
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Добавить новую запись
        /// </summary>
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        /// <summary>
        /// Добавить несколько записей
        /// </summary>
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        /// <summary>
        /// Обновить запись
        /// </summary>
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        /// <summary>
        /// Удалить запись
        /// </summary>
        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        /// <summary>
        /// Удалить несколько записей
        /// </summary>
        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        /// <summary>
        /// Сохранить изменения в базе данных
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}