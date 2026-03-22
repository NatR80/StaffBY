using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffBY.App.ViewModels
{
    /// <summary>
    /// Модель для отображения отпуска в таблице
    /// </summary>
    public class VacationViewModel
    {
        public int Id { get; set; }
        public string EmployeeFullName { get; set; } = string.Empty;
        public string VacationType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DaysCount { get; set; }
        public string Basis { get; set; } = string.Empty;
    }
}
