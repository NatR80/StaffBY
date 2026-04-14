using System;
using System.Collections.Generic;
using System.Windows;
using StaffBY.App.ViewModels;
using StaffBY.App.Views.UserControls.EmployeeCard;

namespace StaffBY.App.Views
{
    public partial class EmployeeEditWindow : Window
    {
        public event EventHandler<EmployeeViewModel>? EmployeeSaved;

        public EmployeeEditWindow(EmployeeViewModel? employee = null, List<PositionViewModel>? positions = null)
        {
            InitializeComponent();

            if (employee == null)
            {
                Title = "Новый сотрудник";
                EmployeeCard.LoadData(new EmployeeViewModel(), positions ?? new List<PositionViewModel>());
            }
            else
            {
                Title = $"Редактирование сотрудника: {employee.FullName}";
                EmployeeCard.LoadData(employee, positions ?? new List<PositionViewModel>());
            }

            EmployeeCard.EmployeeSaved += (s, savedEmployee) =>
            {
                EmployeeSaved?.Invoke(this, savedEmployee);
                DialogResult = true;
                Close();
            };
        }
    }
}