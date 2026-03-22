using System;

namespace StaffBY.Domain.Entities
{
    /// <summary>
    /// Отпуск сотрудника (раздел 6 формы Т-2)
    /// </summary>
    public class Vacation
    {
        public int Id { get; set; }

        // Связь с сотрудником
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        // Период отпуска
        public DateTime StartDate { get; set; }                     // Дата начала
        public DateTime EndDate { get; set; }                       // Дата окончания

        // Вид отпуска (основной, дополнительный, социальный, учебный)
        public string VacationType { get; set; } = string.Empty;

        // Количество дней (рассчитывается автоматически или вводится вручную)
        public int DaysCount { get; set; }

        // За какой рабочий год предоставляется отпуск
        public string WorkPeriod { get; set; } = string.Empty;      // Например: "2025-2026"

        // Основание (приказ)
        public string? OrderNumber { get; set; }
        public DateTime? OrderDate { get; set; }

        /// <summary>
        /// Проверка, не пересекаются ли даты (будем использовать при добавлении)
        /// </summary>
        public bool OverlapsWith(DateTime start, DateTime end)
        {
            return StartDate < end && EndDate > start;
        }
    }
}