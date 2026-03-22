using System;
using System.Collections.Generic;

namespace StaffBY.Domain.Entities
{
    /// <summary>
    /// Сотрудник - центральная сущность системы
    /// Соответствует форме Т-2 "Личная карточка"
    /// </summary>
    public class Employee
    {
        public int Id { get; set; }

        // ----- Раздел 1. Общие сведения -----

        // ФИО сотрудника
        public string LastName { get; set; } = string.Empty;      // Фамилия
        public string FirstName { get; set; } = string.Empty;     // Имя
        public string? Patronymic { get; set; }                   // Отчество (может отсутствовать)

        // Дата рождения и место
        public DateTime BirthDate { get; set; }
        public string? BirthPlace { get; set; }                   // Место рождения

        // Гражданство
        public string? Citizenship { get; set; }                  // По умолчанию "Республика Беларусь"

        // Пол (true - мужской, false - женский)
        public bool IsMale { get; set; } = true;

        // Личный номер (идентификационный номер в РБ)
        public string? PersonalNumber { get; set; }

        // Паспортные данные
        public string? PassportSeries { get; set; }               // Серия паспорта
        public string? PassportNumber { get; set; }               // Номер паспорта
        public string? PassportIssuedBy { get; set; }             // Кем выдан
        public DateTime? PassportIssueDate { get; set; }          // Дата выдачи

        // Контактная информация
        public string? HomeAddress { get; set; }                  // Домашний адрес
        public string? Phone { get; set; }                        // Телефон
        public string? Email { get; set; }                        // Электронная почта

        // Путь к фотографии (храним файл отдельно, в БД только путь)
        public string? PhotoPath { get; set; }

        // Семейное положение (значение из справочника)
        public string? MaritalStatus { get; set; }                // Например: "Женат/Замужем", "Холост" и т.д.

        // Служебные поля
        public bool IsArchived { get; set; } = false;             // true - сотрудник уволен и в архиве
        public DateTime CreatedAt { get; set; } = DateTime.Now;   // Дата создания записи

        // ----- Связи с другими таблицами -----

        // Подразделение, где работает сотрудник
        public int? DepartmentId { get; set; }                    // Внешний ключ
        public Department? Department { get; set; }               // Навигационное свойство

        // Должность сотрудника
        public int? PositionId { get; set; }                       // Внешний ключ
        public Position? Position { get; set; }                    // Навигационное свойство

        // Связанные данные (списки)
        public ICollection<FamilyMember> FamilyMembers { get; set; } = new List<FamilyMember>();
        public ICollection<Education> Educations { get; set; } = new List<Education>();
        public ICollection<EmploymentHistory> EmploymentHistories { get; set; } = new List<EmploymentHistory>();
        public ICollection<Vacation> Vacations { get; set; } = new List<Vacation>();

        // ----- Вспомогательные методы -----

        /// <summary>
        /// Получить полное имя сотрудника (Фамилия Имя Отчество)
        /// </summary>
        public string GetFullName()
        {
            if (string.IsNullOrWhiteSpace(Patronymic))
                return $"{LastName} {FirstName}";
            return $"{LastName} {FirstName} {Patronymic}";
        }

        /// <summary>
        /// Получить фамилию с инициалами (например: Иванов И.И.)
        /// </summary>
        public string GetShortName()
        {
            var firstNameInitial = !string.IsNullOrEmpty(FirstName) ? FirstName[0] + "." : "";
            var patronymicInitial = !string.IsNullOrEmpty(Patronymic) ? Patronymic[0] + "." : "";

            if (string.IsNullOrEmpty(patronymicInitial))
                return $"{LastName} {firstNameInitial}";
            else
                return $"{LastName} {firstNameInitial}{patronymicInitial}";
        }

        /// <summary>
        /// Рассчитать возраст сотрудника
        /// </summary>
        public int GetAge()
        {
            var today = DateTime.Today;
            var age = today.Year - BirthDate.Year;
            // Проверка, был ли уже день рождения в этом году
            if (BirthDate.Date > today.AddYears(-age))
                age--;
            return age;
        }
    }
}