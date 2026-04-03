using System;
using System.Windows;
using StaffBY.App.ViewModels;
using System.Collections.Generic;
using StaffBY.App.Models;

namespace StaffBY.App.Views
{
    public partial class EmployeeEditWindow : Window
    {
        private EmployeeViewModel _employee;
        private bool _isNewEmployee;

        public event EventHandler<EmployeeViewModel>? EmployeeSaved;

        public EmployeeEditWindow(EmployeeViewModel? employee = null)
        {
            InitializeComponent();

            if (employee == null)
            {
                _isNewEmployee = true;
                _employee = new EmployeeViewModel
                {
                    PersonalNumber = GeneratePersonalNumber()
                };
                Title = "Добавление нового сотрудника";
            }
            else
            {
                _isNewEmployee = false;
                _employee = employee;
                Title = $"Редактирование сотрудника: {employee.LastName} {employee.FirstName}";
            }

            EmployeeCard.LoadData(_employee);
            EmployeeCard.EmployeeSaved += EmployeeCard_EmployeeSaved;
        }

        private string GeneratePersonalNumber()
        {
            return DateTime.Now.ToString("yyMMddHHmmss");
        }

        private void EmployeeCard_EmployeeSaved(object? sender, EmployeeViewModel e)
        {
            EmployeeSaved?.Invoke(this, e);
            DialogResult = true;
            Close();
        }
    }
}