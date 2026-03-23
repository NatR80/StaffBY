using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StaffBY.App.ViewModels;
using StaffBY.Domain.Entities;
using StaffBY.Business.Interfaces;  
using Microsoft.Extensions.DependencyInjection;

namespace StaffBY.App.Views
{
    public partial class MainWindow : Window
    {
        private User _currentUser;
        private IEmployeeService? _employeeService;  // Сделали nullable

        private List<EmployeeViewModel> _employees = new List<EmployeeViewModel>();
        private List<PositionViewModel> _positions = new List<PositionViewModel>();
        private List<VacationViewModel> _vacations = new List<VacationViewModel>();

        public MainWindow(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;

            // Альтернативный способ получить сервис
            try
            {
                var app = Application.Current as App;
                if (app != null)
                {
                    _employeeService = app.Services.GetService(typeof(IEmployeeService)) as IEmployeeService;
                }
            }
            catch
            {
                _employeeService = null;
            }

            UserInfoText.Text = $"Пользователь: {currentUser.Username}";

            LoadDemoData();
            RefreshAllData();

            StatusText.Text = $"Добро пожаловать, {currentUser.Username}!";
        }
        //public MainWindow(User currentUser)
        //{
        //    InitializeComponent();
        //    _currentUser = currentUser;

        //    // Правильный способ получить сервис
        //    try
        //    {
        //        if (Application.Current is App app && app.Services != null)
        //        {
        //            _employeeService = app.Services.GetRequiredService<IEmployeeService>();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Ошибка получения сервиса: {ex.Message}");
        //        _employeeService = null;
        //    }

        //    UserInfoText.Text = $"Пользователь: {currentUser.Username}";

        //    LoadDemoData();
        //    RefreshAllData();

        //    StatusText.Text = $"Добро пожаловать, {currentUser.Username}!";
        //}

        //// ... остальные методы остаются без изменений

               
        

        private void LoadDemoData()
        {
            _employees = new List<EmployeeViewModel>
            {
                new EmployeeViewModel
                {
                    Id = 1,
                    PersonalNumber = "001",
                    LastName = "Иванов",
                    FirstName = "Иван",
                    Patronymic = "Иванович",
                    PositionName = "Директор",
                    DepartmentName = "Руководство",
                    BirthDate = new DateTime(1980, 5, 15),
                    Phone = "+375291234567"
                },
                new EmployeeViewModel
                {
                    Id = 2,
                    PersonalNumber = "002",
                    LastName = "Петрова",
                    FirstName = "Анна",
                    Patronymic = "Сергеевна",
                    PositionName = "Главный бухгалтер",
                    DepartmentName = "Бухгалтерия",
                    BirthDate = new DateTime(1985, 8, 22),
                    Phone = "+375292345678"
                },
                new EmployeeViewModel
                {
                    Id = 3,
                    PersonalNumber = "003",
                    LastName = "Сидоров",
                    FirstName = "Петр",
                    Patronymic = "Алексеевич",
                    PositionName = "Инженер-программист",
                    DepartmentName = "IT отдел",
                    BirthDate = new DateTime(1990, 3, 10),
                    Phone = "+375293456789"
                }
            };

            _positions = new List<PositionViewModel>
            {
                new PositionViewModel { Id = 1, Name = "Директор", DepartmentName = "Руководство", Salary = 5000, NumberOfPositions = 1, Allowance = 1000 },
                new PositionViewModel { Id = 2, Name = "Главный бухгалтер", DepartmentName = "Бухгалтерия", Salary = 3500, NumberOfPositions = 1, Allowance = 500 },
                new PositionViewModel { Id = 3, Name = "Инженер-программист", DepartmentName = "IT отдел", Salary = 2500, NumberOfPositions = 3, Allowance = 300 },
                new PositionViewModel { Id = 4, Name = "Менеджер по персоналу", DepartmentName = "Отдел кадров", Salary = 2000, NumberOfPositions = 1, Allowance = 200 }
            };

            _vacations = new List<VacationViewModel>
            {
                new VacationViewModel
                {
                    Id = 1,
                    EmployeeFullName = "Иванов Иван Иванович",
                    VacationType = "Основной",
                    StartDate = new DateTime(2026, 6, 1),
                    EndDate = new DateTime(2026, 6, 28),
                    DaysCount = 28,
                    Basis = "Приказ №123 от 01.05.2026"
                }
            };
        }

        private void RefreshAllData()
        {
            RefreshEmployeesGrid();
            RefreshPositionsGrid();
            RefreshVacationsGrid();
            RefreshArchivedEmployeesGrid();
            RefreshEmployeeComboBox();
        }

        private void RefreshEmployeesGrid()
        {
            EmployeesDataGrid.ItemsSource = _employees.Where(e => !e.IsArchived).ToList();
            RecordCountText.Text = $"Записей: {EmployeesDataGrid.ItemsSource.Cast<object>().Count()}";
        }

        private void RefreshPositionsGrid()
        {
            PositionsDataGrid.ItemsSource = _positions.ToList();
        }

        private void RefreshVacationsGrid()
        {
            VacationsDataGrid.ItemsSource = _vacations.ToList();
        }

        private void RefreshArchivedEmployeesGrid()
        {
            ArchivedEmployeesDataGrid.ItemsSource = _employees.Where(e => e.IsArchived).ToList();
        }

        private void RefreshEmployeeComboBox()
        {
            var activeEmployees = _employees.Where(e => !e.IsArchived).ToList();
            EmployeeComboBox.ItemsSource = activeEmployees;

            if (activeEmployees.Count > 0)
                EmployeeComboBox.SelectedIndex = 0;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
                var authWindow = new AuthWindow();
                authWindow.Show();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                RefreshEmployeesGrid();
            }
            else
            {
                var filtered = _employees
                    .Where(e => !e.IsArchived &&
                        (e.LastName.ToLower().Contains(searchText) ||
                         e.FirstName.ToLower().Contains(searchText) ||
                         e.Patronymic.ToLower().Contains(searchText) ||
                         e.PersonalNumber.Contains(searchText)))
                    .ToList();

                EmployeesDataGrid.ItemsSource = filtered;
                RecordCountText.Text = $"Записей: {filtered.Count} (найдено)";
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox_TextChanged(sender, null);
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EmployeeEditWindow();
            editWindow.EmployeeSaved += (s, employee) =>
            {
                employee.Id = _employees.Count > 0 ? _employees.Max(x => x.Id) + 1 : 1;
                _employees.Add(employee);
                RefreshEmployeesGrid();
                StatusText.Text = $"Сотрудник {employee.LastName} {employee.FirstName} добавлен";
            };
            editWindow.Owner = this;
            editWindow.ShowDialog();
        }

        private void EmployeesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (EmployeesDataGrid.SelectedItem is EmployeeViewModel selectedEmployee)
            {
                var editWindow = new EmployeeEditWindow(selectedEmployee);
                editWindow.EmployeeSaved += (s, employee) =>
                {
                    var index = _employees.FindIndex(x => x.Id == employee.Id);
                    if (index >= 0)
                    {
                        _employees[index] = employee;
                        RefreshEmployeesGrid();
                        StatusText.Text = $"Сотрудник {employee.LastName} {employee.FirstName} обновлен";
                    }
                };
                editWindow.Owner = this;
                editWindow.ShowDialog();
            }
        }

        // Остальные методы оставляем как были...
        private void ExportToExcelButton_Click(object sender, RoutedEventArgs e) { }
        private void AddPositionButton_Click(object sender, RoutedEventArgs e) { }
        private void EditPositionButton_Click(object sender, RoutedEventArgs e) { }
        private void DeletePositionButton_Click(object sender, RoutedEventArgs e) { }
        private void AddVacationButton_Click(object sender, RoutedEventArgs e) { }
        private void VacationScheduleButton_Click(object sender, RoutedEventArgs e) { }
        private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
        private void GenerateDocumentButton_Click(object sender, RoutedEventArgs e) { }
        private void EmployeeListReportButton_Click(object sender, RoutedEventArgs e) { }
        private void PersonnelMovementReportButton_Click(object sender, RoutedEventArgs e) { }
        private void BirthdaysReportButton_Click(object sender, RoutedEventArgs e) { }
        private void VacationsReportButton_Click(object sender, RoutedEventArgs e) { }
        private void MilitaryReportButton_Click(object sender, RoutedEventArgs e) { }
        private string GenerateDocument(string? docType, EmployeeViewModel? employee) => "";
    }
}