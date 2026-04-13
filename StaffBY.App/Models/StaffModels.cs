using StaffBY.App.ViewModels.StaffBY.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace StaffBY.App.Models
{
    /// <summary>
    /// Подразделение (вместо "Отдел")
    /// </summary>
    public class DepartmentModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentDepartmentId { get; set; } // для иерархии (null = главное)
        public List<DepartmentModel> ChildDepartments { get; set; } = new();
        public List<PositionViewModel> Positions { get; set; } = new();
    }

    /// <summary>
    /// Должность в штатном расписании
    /// </summary>
    //public class PositionInStaffModel
    //{
    //    public int Id { get; set; }

    //    // Основные
    //    public string Name { get; set; } = string.Empty;           // Должность
    //    public string DepartmentName { get; set; } = string.Empty; // Подразделение

    //    // Количество
    //    public int StaffUnits { get; set; } = 1;      // Кол-во штатных единиц
    //    public int FilledPositions { get; set; } = 0; // Занято (подсчет из сотрудников)
    //    public int Vacancies => StaffUnits - FilledPositions; // Вакансии (авто)

    //    // Условия работы
    //    public decimal Rate { get; set; } = 1.0m;     // Ставка (0.25, 0.5, 0.75, 1.0)
    //    public string WorkWeek { get; set; } = "5-дневная"; // Рабочая неделя
    //    public string Category { get; set; } = "Специалист"; // Категория
    //    public decimal Salary { get; set; }          // Оклад
    //    public decimal Allowance { get; set; }       // Надбавка
    //}
}
