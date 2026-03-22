using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using StaffBY.DAL.Configuration;

namespace StaffBY.DAL.Context
{
    /// <summary>
    /// Фабрика для создания контекста базы данных во время разработки
    /// Нужна для инструментов EF Core (миграции)
    /// </summary>
    public class StaffBYDbContextFactory : IDesignTimeDbContextFactory<StaffBYDbContext>
    {
        public StaffBYDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<StaffBYDbContext>();

            // Получаем строку подключения из конфигурации
            string connectionString = DatabaseConfig.GetConnectionString();

            // Настраиваем использование SQL Server
            optionsBuilder.UseSqlServer(connectionString);

            return new StaffBYDbContext(optionsBuilder.Options);
        }
    }
}