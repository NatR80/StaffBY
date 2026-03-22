using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace StaffBY.Business.Services
{
    /// <summary>
    /// Сервис для хеширования и проверки паролей
    /// </summary>
    public static class PasswordHasher
    {
        /// <summary>
        /// Создает хеш пароля (SHA-256)
        /// </summary>
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(hashedBytes).ToLower();
        }

        /// <summary>
        /// Проверяет, совпадает ли пароль с хешем
        /// </summary>
        public static bool VerifyPassword(string password, string hash)
        {
            var hashedInput = HashPassword(password);
            return hashedInput.Equals(hash, StringComparison.OrdinalIgnoreCase);
        }
    }
}