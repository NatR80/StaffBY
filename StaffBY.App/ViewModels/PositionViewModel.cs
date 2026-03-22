using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace StaffBY.App.ViewModels
{
    /// <summary>
    /// Модель для отображения должности в таблице
    /// </summary>
    public class PositionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public int NumberOfPositions { get; set; }
        public decimal Salary { get; set; }
        public decimal Allowance { get; set; }
    }
}