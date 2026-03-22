using System;

namespace StaffBY.Domain.Entities
{
    /// <summary>
    /// Кадровая история (назначения и перемещения)
    /// Раздел 3 формы Т-2
    /// </summary>
    public class EmploymentHistory
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public DateTime StartDate { get; set; }                     // Дата назначения
        public DateTime? EndDate { get; set; }                      // Дата освобождения (null если текущая)

        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        public int PositionId { get; set; }
        public Position Position { get; set; } = null!;

        public decimal Salary { get; set; }                         // Оклад/тариф

        public string? OrderNumber { get; set; }                    // Номер приказа
        public DateTime? OrderDate { get; set; }                    // Дата приказа

        public bool IsCurrent => EndDate == null;                   // Текущая запись
    }
}
