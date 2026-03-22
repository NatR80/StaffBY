using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StaffBY.Domain.Entities;
using System.Threading.Tasks;

namespace StaffBY.Business.Interfaces
{
    /// <summary>
    /// Сервис для работы с пользователями системы
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Проверка логина и пароля пользователя
        /// </summary>
        /// <param name="username">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns>Пользователь, если аутентификация успешна, иначе null</returns>
        Task<User?> AuthenticateAsync(string username, string password);

        /// <summary>
        /// Получить пользователя по логину
        /// </summary>
        Task<User?> GetByUsernameAsync(string username);
    }
}
