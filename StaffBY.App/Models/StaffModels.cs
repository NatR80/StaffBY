using StaffBY.App.ViewModels;
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

    
}
