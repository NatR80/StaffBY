using System;
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

        public void LoadData(EmployeeViewModel employee)
        {
            _employee = employee ?? throw new ArgumentNullException(nameof(employee));
            txtTitle.Text = $"ЛИЧНАЯ КАРТОЧКА РАБОТНИКА (форма Т-2) - {employee.FullName}";

            CommonInfo.LoadData(employee);
            Employment.LoadData(employee);
            Military.LoadData(employee);
            Vacations.LoadData(employee);
            Family.LoadData(employee?.FamilyMembers ?? new System.Collections.Generic.List<FamilyMemberEntry>());
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_employee == null) return;

            CommonInfo.SaveData();
            Employment.SaveData();
            Military.SaveData();
            Vacations.SaveData();
            _employee.FamilyMembers = Family.GetFamilyMembers();

            // Просто вызываем событие, НЕ закрываем окно
            EmployeeSaved?.Invoke(this, _employee);

            // Убираем закрытие окна!
            // var window = Window.GetWindow(this);
            // window?.Close();

            // Показываем сообщение об успешном сохранении
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