using System;
using System.IO;

namespace StaffBY.DAL.Configuration
{
    /// <summary>
    /// Класс для настройки подключения к базе данных
    /// </summary>
    public static class DatabaseConfig
    {
        /// <summary>
        /// Получить строку подключения к базе данных
        /// </summary>
        public static string GetConnectionString()
        {
            // Вариант 1: Подключение к локальному SQL Server Express
            // return "Server=.\\SQLEXPRESS;Database=StaffBYDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";

            // Вариант 2: Подключение к локальному SQL Server (стандартный)
            // return "Server=localhost;Database=StaffBYDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";

            // Вариант 3: Подключение к SQL Server с аутентификацией SQL (замените логин/пароль)
            // return "Server=localhost;Database=StaffBYDB;User Id=sa;Password=your_password;MultipleActiveResultSets=true;TrustServerCertificate=True;";

            // Вариант 4: Использование базы данных в файле (LocalDB - для разработки)
            // LocalDB - легковесная база, которая устанавливается вместе с Visual Studio
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StaffBYDB.mdf");
            return $@"Server=(localdb)\MSSQLLocalDB;Database=StaffBYDB;Integrated Security=true;MultipleActiveResultSets=true;TrustServerCertificate=True;";
                        

            // Примечание: если у вас нет LocalDB, используйте вариант 1 или 2
        }
    }
}