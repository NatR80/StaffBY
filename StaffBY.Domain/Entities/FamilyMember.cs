using System;

namespace StaffBY.Domain.Entities
{
    /// <summary>
    /// Член семьи сотрудника (состав семьи)
    /// Раздел 1, пункт 14 формы Т-2
    /// </summary>
    public class FamilyMember
    {
        public int Id { get; set; }

        // Связь с сотрудником
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public string FullName { get; set; } = string.Empty;      // ФИО члена семьи
        public int BirthYear { get; set; }                         // Год рождения
        public string Relationship { get; set; } = string.Empty;   // Степень родства (жена, муж, сын, дочь)
    }
}