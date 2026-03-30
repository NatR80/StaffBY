using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using StaffBY.App.ViewModels;

namespace StaffBY.App.Views.UserControls
{
    public partial class VacationsControl : UserControl
    {
        private List<VacationViewModel> _vacations = new List<VacationViewModel>();
        public List<VacationViewModel> GetVacations() => _vacations;
        public event Action<string>? StatusMessageChanged;

        public VacationsControl()
        {
            InitializeComponent();
            LoadDemoData();
            LoadYears();
        }

        private void LoadDemoData()
        {
            _vacations = new List<VacationViewModel>
            {
                new VacationViewModel { Id = 1, EmployeeFullName = "Иванов Иван Иванович", VacationType = "Основной",
                    StartDate = new DateTime(2026, 6, 1), EndDate = new DateTime(2026, 6, 28), DaysCount = 28, Basis = "Приказ №123" }
            };
            VacationsDataGrid.ItemsSource = _vacations;
        }

        private void LoadYears()
        {
            for (int i = 2024; i <= 2030; i++)
                YearComboBox.Items.Add(i);
            YearComboBox.SelectedIndex = 2;
        }

        private void AddVacationButton_Click(object sender, RoutedEventArgs e) =>
            StatusMessageChanged?.Invoke("Добавление отпуска...");

        private void VacationScheduleButton_Click(object sender, RoutedEventArgs e) =>
            StatusMessageChanged?.Invoke("График отпусков...");

        
    }
}