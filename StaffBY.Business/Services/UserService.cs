using Microsoft.EntityFrameworkCore;
using StaffBY.Business.Interfaces;
using StaffBY.DAL.Context;
using StaffBY.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace StaffBY.Business.Services
{
    public class UserService : IUserService
    {
        private readonly StaffBYDbContext _context;

        public UserService(StaffBYDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Аутентификация пользователя
        /// </summary>
        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            // Ищем пользователя по логину
            var user = await _context.Users
                .Include(u => u.Employee) // Загружаем связанного сотрудника
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            if (user == null)
                return null;

            // Проверяем пароль
            var hashedPassword = PasswordHasher.HashPassword(password);
            if (user.PasswordHash != hashedPassword)
                return null;

            // Обновляем дату последнего входа
            user.LastLoginAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return user;
        }

        

        /// <summary>
        /// Получить пользователя по логину
        /// </summary>
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}