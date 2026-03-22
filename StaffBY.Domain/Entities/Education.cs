using System;

namespace StaffBY.Domain.Entities
{
    /// <summary>
    /// Образование сотрудника
    /// Раздел 1, пункты 5-7 формы Т-2
    /// </summary>
    public class Education
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public string InstitutionName { get; set; } = string.Empty; // Название учебного заведения
        public DateTime GraduationDate { get; set; }                // Дата окончания
        public string Specialty { get; set; } = string.Empty;       // Специальность по диплому
        public string Qualification { get; set; } = string.Empty;   // Квалификация по диплому
        public string DiplomaNumber { get; set; } = string.Empty;   // Номер диплома
        public string EducationLevel { get; set; } = string.Empty;  // Уровень (высшее, среднее и т.д.)
    }
}