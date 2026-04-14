using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffBY.App.ViewModels
{
    /// <summary>
    /// Модель для отображения должности в штатном расписании
    /// </summary>
    public class PositionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public int NumberOfPositions { get; set; }  // Штатных единиц

        public int FilledPositions { get; set; } = 0;  // Занято
        public int Vacancies => NumberOfPositions - FilledPositions;  // Вакансии (авто)
        public decimal Rate { get; set; } = 1.0m;  // Ставка
        public string WorkWeek { get; set; } = "5-дневная";  // Рабочая неделя
        public string Category { get; set; } = "Специалист";  // Категория
        public decimal Salary { get; set; }  // Оклад
        public decimal Allowance { get; set; }  // Надбавка
    }
}