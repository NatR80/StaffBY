using System;
using StaffBY.Domain.Enums;

namespace StaffBY.Domain.Entities
{
    /// <summary>
    /// Пользователь системы (для входа и разграничения прав)
    /// </summary>
    public class User
    {
        public int Id { get; set; }

        // Учетные данные
        public string Username { get; set; } = string.Empty;       // Логин для входа
        public string PasswordHash { get; set; } = string.Empty;   // Хеш пароля (никогда не храним пароль в открытом виде!)

        // Роль пользователя (админ, кадровик, бухгалтер, экономист)
        public UserRole Role { get; set; }

        // Связь с сотрудником (если пользователь является сотрудником)
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        // Активна ли учетная запись
        public bool IsActive { get; set; } = true;

        // Дата последнего входа
        public DateTime? LastLoginAt { get; set; }

        // Дата создания
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
