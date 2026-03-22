using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StaffBY.DAL.Interfaces
{
    /// <summary>
    /// Базовый интерфейс репозитория для работы с любыми сущностями
    /// </summary>
    /// <typeparam name="T">Тип сущности</typeparam>
    public interface IRepository<T> where T : class
    {
        // Получить все записи
        Task<IEnumerable<T>> GetAllAsync();

        // Получить запись по id
        Task<T?> GetByIdAsync(int id);

        // Найти записи по условию
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        // Добавить новую запись
        Task AddAsync(T entity);

        // Добавить несколько записей
        Task AddRangeAsync(IEnumerable<T> entities);

        // Обновить запись
        void Update(T entity);

        // Удалить запись
        void Remove(T entity);

        // Удалить несколько записей
        void RemoveRange(IEnumerable<T> entities);

        // Сохранить изменения в БД
        Task<int> SaveChangesAsync();
    }
}