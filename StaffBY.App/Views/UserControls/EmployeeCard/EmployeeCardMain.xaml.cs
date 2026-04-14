using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using StaffBY.App.ViewModels;
using StaffBY.App.Models;


namespace StaffBY.App.Views.UserControls.EmployeeCard
{
    public partial class EmployeeCardMain : UserControl
    {
        private EmployeeViewModel? _employee;

        public event EventHandler<EmployeeViewModel>? EmployeeSaved;

        public EmployeeCardMain()
        {
            InitializeComponent();
        }

        public void LoadData(EmployeeViewModel employee, List<PositionViewModel> positions)
        {
            _employee = employee ?? throw new ArgumentNullException(nameof(employee));
            txtTitle.Text = $"ЛИЧНАЯ КАРТОЧКА РАБОТНИКА (форма Т-2) - {employee.FullName}";

            CommonInfo.LoadData(employee);
            Employment.LoadPositions(positions);
            Employment.LoadData(employee);
            Military.LoadData(employee);
            Vacations.LoadData(employee);
            Family.LoadData(employee?.FamilyMembers ?? new List<FamilyMemberEntry>());
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_employee == null) return;

            CommonInfo.SaveData();
            Employment.SaveData();
            Military.SaveData();
            Vacations.SaveData();
            _employee.FamilyMembers = Family.GetFamilyMembers();

            EmployeeSaved?.Invoke(this, _employee);

            MessageBox.Show($"Данные сотрудника {_employee.FullName} успешно сохранены!",
                "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window?.Close();
        }
    }
}