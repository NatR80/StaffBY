using System.Collections.Generic;

namespace StaffBY.Domain.Entities
{
    /// <summary>
    /// Должность
    /// </summary>
    public class Position
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty; // Наименование должности
        public decimal? BaseSalary { get; set; } // Базовый оклад
        public string? Requirements { get; set; } // Требования к должности

        // Навигационные свойства
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
