using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StaffBY.App.ViewModels;

namespace StaffBY.App.Views.UserControls
{
    public partial class PositionsControl : UserControl
    {
        private List<PositionViewModel> _positions = new List<PositionViewModel>();
        public event Action<string>? StatusMessageChanged;

        public PositionsControl()
        {
            InitializeComponent();
            LoadDemoData();
        }

        private void LoadDemoData()
        {
            _positions = new List<PositionViewModel>
            {
                new PositionViewModel { Id = 1, Name = "Директор", DepartmentName = "Руководство", Salary = 5000, NumberOfPositions = 1, Allowance = 1000 },
                new PositionViewModel { Id = 2, Name = "Главный бухгалтер", DepartmentName = "Бухгалтерия", Salary = 3500, NumberOfPositions = 1, Allowance = 500 },
                new PositionViewModel { Id = 3, Name = "Инженер-программист", DepartmentName = "IT отдел", Salary = 2500, NumberOfPositions = 3, Allowance = 300 }
            };
            RefreshGrid();
        }

        private void RefreshGrid() => PositionsDataGrid.ItemsSource = _positions;

        private void AddPositionButton_Click(object sender, RoutedEventArgs e)
        {
            StatusMessageChanged?.Invoke("Добавление должности...");
            MessageBox.Show("Функция добавления должности в разработке", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditPositionButton_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("Функция редактирования должности в разработке", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

        private void DeletePositionButton_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("Функция удаления должности в разработке", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}