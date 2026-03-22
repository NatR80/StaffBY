using Microsoft.EntityFrameworkCore;
using StaffBY.Domain.Entities;

namespace StaffBY.DAL.Context
{
    /// <summary>
    /// Контекст базы данных для работы с Entity Framework
    /// Этот класс связывает наши сущности с таблицами в БД
    /// </summary>
    public class StaffBYDbContext : DbContext
    {
        // Конструктор, принимающий параметры подключения
        public StaffBYDbContext(DbContextOptions<StaffBYDbContext> options)
            : base(options)
        {
        }

        // DbSet представляет таблицу в базе данных
        // Каждое свойство - это отдельная таблица
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<FamilyMember> FamilyMembers { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<EmploymentHistory> EmploymentHistories { get; set; }
        public DbSet<Vacation> Vacations { get; set; }
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Настройка связей между таблицами и дополнительных ограничений
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === Настройка таблицы Employees (Сотрудники) ===
            modelBuilder.Entity<Employee>(entity =>
            {
                // Указываем, что поле LastName обязательно и имеет максимальную длину
                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Patronymic)
                    .HasMaxLength(50);

                // Индекс для быстрого поиска по фамилии
                entity.HasIndex(e => e.LastName)
                    .HasDatabaseName("IX_Employees_LastName");

                // Индекс для поиска по личному номеру (уникальный)
                entity.HasIndex(e => e.PersonalNumber)
                    .IsUnique()
                    .HasDatabaseName("IX_Employees_PersonalNumber")
                    .HasFilter("PersonalNumber IS NOT NULL"); // Только для непустых значений

                // Связь с Department (один ко многим)
                entity.HasOne(e => e.Department)
                    .WithMany(d => d.Employees)
                    .HasForeignKey(e => e.DepartmentId)
                    .OnDelete(DeleteBehavior.SetNull); // При удалении отдела сотрудники остаются, но без отдела

                // Связь с Position (один ко многим)
                entity.HasOne(e => e.Position)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(e => e.PositionId)
                    .OnDelete(DeleteBehavior.SetNull); // При удалении должности сотрудники остаются
            });

            // === Настройка таблицы Departments (Подразделения) ===
            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(d => d.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                // Название отдела должно быть уникальным
                entity.HasIndex(d => d.Name)
                    .IsUnique()
                    .HasDatabaseName("IX_Departments_Name");
            });

            // === Настройка таблицы Positions (Должности) ===
            modelBuilder.Entity<Position>(entity =>
            {
                entity.Property(p => p.Title)
                    .IsRequired()
                    .HasMaxLength(100);

                // Название должности должно быть уникальным
                entity.HasIndex(p => p.Title)
                    .IsUnique()
                    .HasDatabaseName("IX_Positions_Title");

                entity.Property(p => p.BaseSalary)     // Добавляем настройку для BaseSalary
                    .HasPrecision(18, 2);

            });

            // === Настройка таблицы FamilyMembers (Члены семьи) ===
            modelBuilder.Entity<FamilyMember>(entity =>
            {
                entity.Property(f => f.FullName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(f => f.Relationship)
                    .IsRequired()
                    .HasMaxLength(50);

                // Связь с Employee
                entity.HasOne(f => f.Employee)
                    .WithMany(e => e.FamilyMembers)
                    .HasForeignKey(f => f.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade); // При удалении сотрудника удаляются и члены семьи
            });

            // === Настройка таблицы Education (Образование) ===
            modelBuilder.Entity<Education>(entity =>
            {
                entity.Property(e => e.InstitutionName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Specialty)
                    .HasMaxLength(100);

                entity.Property(e => e.Qualification)
                    .HasMaxLength(100);

                entity.Property(e => e.DiplomaNumber)
                    .HasMaxLength(50);

                // Связь с Employee
                entity.HasOne(e => e.Employee)
                    .WithMany(emp => emp.Educations)
                    .HasForeignKey(e => e.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // === Настройка таблицы EmploymentHistory (Кадровая история) ===
            modelBuilder.Entity<EmploymentHistory>(entity =>
            {
                // Связь с Employee
                entity.HasOne(h => h.Employee)
                    .WithMany(e => e.EmploymentHistories)
                    .HasForeignKey(h => h.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);
                

                // Связь с Department
                entity.HasOne(h => h.Department)
                    .WithMany()
                    .HasForeignKey(h => h.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict); // Запрещаем удаление отдела, если есть история

                // Связь с Position
                entity.HasOne(h => h.Position)
                    .WithMany()
                    .HasForeignKey(h => h.PositionId)
                    .OnDelete(DeleteBehavior.Restrict); // Запрещаем удаление должности, если есть история


                // Настройка для Salary (зарплата в истории) - здесь правильно
                entity.Property(e => e.Salary)
                    .HasPrecision(18, 2);

                // Индекс для поиска по датам
                entity.HasIndex(h => h.StartDate)
                    .HasDatabaseName("IX_EmploymentHistory_StartDate");

                
            });

            // === Настройка таблицы Vacations (Отпуска) ===
            modelBuilder.Entity<Vacation>(entity =>
            {
                entity.Property(v => v.VacationType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(v => v.WorkPeriod)
                    .HasMaxLength(20);

                // Связь с Employee
                entity.HasOne(v => v.Employee)
                    .WithMany(e => e.Vacations)
                    .HasForeignKey(v => v.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Проверка, что дата окончания позже даты начала
                entity.ToTable(t => t.HasCheckConstraint("CK_Vacation_EndDateGreaterThanStartDate",
                    "[EndDate] >= [StartDate]"));
            });

            // === Настройка таблицы Users (Пользователи системы) ===
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                // Логин должен быть уникальным
                entity.HasIndex(u => u.Username)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Username");

                entity.Property(u => u.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(256); // Для хеша пароля

                // Связь с Employee (один к одному)
                entity.HasOne(u => u.Employee)
                    .WithOne()
                    .HasForeignKey<User>(u => u.EmployeeId)
                    .OnDelete(DeleteBehavior.SetNull); // При удалении сотрудника пользователь остается, но без связи
            });

            // Добавим начальные данные (seed data) для справочников
            SeedData(modelBuilder);
        }

        /// <summary>
        /// Заполнение базы начальными данными (для тестирования)
        /// </summary>
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Добавляем несколько отделов по умолчанию
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "Администрация", Description = "Руководство предприятия", CreatedAt = DateTime.Now },
                new Department { Id = 2, Name = "Отдел кадров", Description = "Управление персоналом", CreatedAt = DateTime.Now },
                new Department { Id = 3, Name = "Бухгалтерия", Description = "Финансовый учет", CreatedAt = DateTime.Now },
                new Department { Id = 4, Name = "IT-отдел", Description = "Информационные технологии", CreatedAt = DateTime.Now }
            );

            // Добавляем несколько должностей по умолчанию
            modelBuilder.Entity<Position>().HasData(
                new Position { Id = 1, Title = "Директор", BaseSalary = 3000 },
                new Position { Id = 2, Title = "Начальник отдела кадров", BaseSalary = 2000 },
                new Position { Id = 3, Title = "Бухгалтер", BaseSalary = 1500 },
                new Position { Id = 4, Title = "Программист", BaseSalary = 1800 },
                new Position { Id = 5, Title = "Инженер", BaseSalary = 1600 }
            );

            // Добавляем тестового администратора (пароль: admin)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918", // SHA256 "admin"
                    Role = Domain.Enums.UserRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
}