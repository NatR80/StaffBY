using System;
using System.Collections.Generic;

namespace StaffBY.Domain.Entities
{
    /// <summary>
    /// Подразделение предприятия
    /// </summary>
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Название отдела
        public string? Description { get; set; } // Описание/функции отдела
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true; // Активно ли подразделение

        // Навигационные свойства
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
