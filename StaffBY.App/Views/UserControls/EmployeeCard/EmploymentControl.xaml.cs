using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StaffBY.App.ViewModels;
using System.Collections.Generic;
using StaffBY.App.Models;


namespace StaffBY.App.Views.UserControls.EmployeeCard
{
    public partial class EmploymentControl : UserControl
    {
        private EmployeeViewModel? _employee;

        public EmploymentControl()
        {
            InitializeComponent();
        }

        public void LoadData(EmployeeViewModel employee)
        {
            _employee = employee;
            if (employee == null) return;

            cmbPosition.Text = employee.PositionName;
            cmbDepartment.Text = employee.DepartmentName;
            dpHireDate.SelectedDate = employee.HireDate;
            txtSalary.Text = employee.Salary?.ToString("N2");
            cmbEmploymentType.Text = employee.EmploymentType;
            cmbContractType.Text = employee.ContractType;
            dpContractEndDate.SelectedDate = employee.ContractEndDate;
        }

        public void SaveData()
        {
            if (_employee == null) return;

            _employee.PositionName = cmbPosition.Text;
            _employee.DepartmentName = cmbDepartment.Text;
            _employee.HireDate = dpHireDate.SelectedDate;

            if (decimal.TryParse(txtSalary.Text, out decimal salary))
                _employee.Salary = salary;

            _employee.EmploymentType = cmbEmploymentType.Text;
            _employee.ContractType = cmbContractType.Text;
            _employee.ContractEndDate = dpContractEndDate.SelectedDate;
        }
    }
}