using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffBY.App.ViewModels
{
    /// <summary>
    /// Модель для отображения сотрудника в таблице
    /// </summary>
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public string PersonalNumber { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public string FullName => $"{LastName} {FirstName} {Patronymic}".Trim();
        public string PositionName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Phone { get; set; } = string.Empty;
        public bool IsArchived { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime? DismissalDate { get; set; }
        public string DismissalReason { get; set; } = string.Empty;
    }
}
