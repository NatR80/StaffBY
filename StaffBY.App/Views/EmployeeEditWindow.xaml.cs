using System;
using System.Windows;
using StaffBY.App.ViewModels;

namespace StaffBY.App.Views
{
    public partial class EmployeeEditWindow : Window
    {
        public event EventHandler<EmployeeViewModel>? EmployeeSaved;

        public EmployeeEditWindow(EmployeeViewModel? employee = null)
        {
            InitializeComponent();

            if (employee == null)
            {
                employee = new EmployeeViewModel
                {
                    PersonalNumber = GeneratePersonalNumber()
                };
                Title = "Добавление нового сотрудника";
            }
            else
            {
                Title = $"Редактирование сотрудника: {employee.LastName} {employee.FirstName}";
            }

            EmployeeCard.LoadData(employee);
            EmployeeCard.EmployeeSaved += EmployeeCard_EmployeeSaved;
        }

        private string GeneratePersonalNumber()
        {
            return DateTime.Now.ToString("yyMMddHHmmss");
        }

        private void EmployeeCard_EmployeeSaved(object? sender, EmployeeViewModel e)
        {
            // Просто вызываем событие, НЕ закрываем окно
            EmployeeSaved?.Invoke(this, e);

            // Убираем Close() - окно остается открытым!
            // DialogResult = true;
            // Close();
        }
    }
}