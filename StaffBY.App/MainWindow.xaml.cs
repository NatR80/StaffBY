using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StaffBY.App.ViewModels;
using StaffBY.Domain.Entities;

namespace StaffBY.App.Views
{
    public partial class MainWindow : Window
    {
        private User _currentUser;

        // Используем ViewModel для отображения
        private List<EmployeeViewModel> _employees = new List<EmployeeViewModel>();
        private List<PositionViewModel> _positions = new List<PositionViewModel>();
        private List<VacationViewModel> _vacations = new List<VacationViewModel>();

        public MainWindow(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;

            UserInfoText.Text = $"Пользователь: {currentUser.Username}";

            LoadDemoData();
            RefreshAllData();

            StatusText.Text = $"Добро пожаловать, {currentUser.Username}!";
        }

        private void LoadDemoData()
        {
            // Демо-сотрудники
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

            // Демо-должности
            _positions = new List<PositionViewModel>
            {
                new PositionViewModel { Id = 1, Name = "Директор", DepartmentName = "Руководство", Salary = 5000, NumberOfPositions = 1, Allowance = 1000 },
                new PositionViewModel { Id = 2, Name = "Главный бухгалтер", DepartmentName = "Бухгалтерия", Salary = 3500, NumberOfPositions = 1, Allowance = 500 },
                new PositionViewModel { Id = 3, Name = "Инженер-программист", DepartmentName = "IT отдел", Salary = 2500, NumberOfPositions = 3, Allowance = 300 },
                new PositionViewModel { Id = 4, Name = "Менеджер по персоналу", DepartmentName = "Отдел кадров", Salary = 2000, NumberOfPositions = 1, Allowance = 200 }
            };

            // Демо-отпуска
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
            RecordCountText.Text = $"Записей: {EmployeesDataGrid.Items.Count}";
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

            if (activeEmployees.Any())
                EmployeeComboBox.SelectedIndex = 0;
        }

        // ---------- Обработчики ----------

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
                // Добавляем нового сотрудника
                employee.Id = _employees.Any() ? _employees.Max(x => x.Id) + 1 : 1;
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
                    // Обновляем данные сотрудника
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

        private void ExportToExcelButton_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Экспорт данных в Excel...";
            MessageBox.Show("Функция экспорта в Excel будет реализована",
                "Информация",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void AddPositionButton_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Открытие формы добавления должности...";
            MessageBox.Show("Форма добавления должности будет открыта здесь",
                "Информация",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void EditPositionButton_Click(object sender, RoutedEventArgs e)
        {
            if (PositionsDataGrid.SelectedItem is PositionViewModel selectedPosition)
            {
                StatusText.Text = $"Редактирование должности: {selectedPosition.Name}";
                MessageBox.Show($"Редактирование должности: {selectedPosition.Name}",
                    "Информация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Выберите должность для редактирования",
                    "Предупреждение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void DeletePositionButton_Click(object sender, RoutedEventArgs e)
        {
            if (PositionsDataGrid.SelectedItem is PositionViewModel selectedPosition)
            {
                var result = MessageBox.Show($"Удалить должность '{selectedPosition.Name}'?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _positions.Remove(selectedPosition);
                    RefreshPositionsGrid();
                    StatusText.Text = $"Должность '{selectedPosition.Name}' удалена";
                }
            }
            else
            {
                MessageBox.Show("Выберите должность для удаления",
                    "Предупреждение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void AddVacationButton_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Открытие формы добавления отпуска...";
            MessageBox.Show("Форма добавления отпуска будет открыта здесь",
                "Информация",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void VacationScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Формирование графика отпусков...";
            MessageBox.Show("График отпусков будет сформирован здесь",
                "Информация",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: Фильтрация отпусков по году
        }

        private void GenerateDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            if (DocumentTypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип документа",
                    "Предупреждение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (EmployeeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите сотрудника",
                    "Предупреждение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var docType = (DocumentTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            var employee = EmployeeComboBox.SelectedItem as EmployeeViewModel;

            string document = GenerateDocument(docType, employee);
            DocumentPreviewText.Text = document;

            StatusText.Text = $"Сформирован документ: {docType} для {employee?.LastName} {employee?.FirstName}";
        }

        private string GenerateDocument(string? docType, EmployeeViewModel? employee)
        {
            if (employee == null) return "Сотрудник не выбран";

            string currentDate = DateTime.Now.ToString("dd.MM.yyyy");

            switch (docType)
            {
                case "Трудовой договор":
                    return $"ТРУДОВОЙ ДОГОВОР №____\n\n" +
                           $"г. Минск                    {currentDate}\n\n" +
                           $"Наниматель: _______________________\n" +
                           $"Работник: {employee.LastName} {employee.FirstName} {employee.Patronymic}\n\n" +
                           $"1. Предмет договора\n" +
                           $"Работник принимается на должность {employee.PositionName}\n" +
                           $"в отдел {employee.DepartmentName}\n\n" +
                           $"2. Срок договора: бессрочный\n\n" +
                           $"3. Условия оплаты труда: должностной оклад согласно штатному расписанию\n\n" +
                           $"Подписи сторон:\n" +
                           $"Наниматель: ______________\n" +
                           $"Работник: ________________";

                case "Приказ о приеме":
                    return $"ПРИКАЗ №____\n\n" +
                           $"г. Минск                    {currentDate}\n\n" +
                           $"О приеме на работу\n\n" +
                           $"Принять на работу:\n" +
                           $"{employee.LastName} {employee.FirstName} {employee.Patronymic}\n" +
                           $"на должность {employee.PositionName}\n" +
                           $"в отдел {employee.DepartmentName}\n\n" +
                           $"Основание: трудовой договор №____ от {currentDate}\n\n" +
                           $"Директор: ______________\n" +
                           $"С приказом ознакомлен: ______________";

                default:
                    return "Шаблон документа не найден";
            }
        }

        private void EmployeeListReportButton_Click(object sender, RoutedEventArgs e)
        {
            string report = "=== СПИСОК СОТРУДНИКОВ ===\n\n";
            report += $"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm}\n";
            report += $"Всего сотрудников: {_employees.Count(e => !e.IsArchived)}\n\n";
            report += "------------------------------------------------\n";
            report += "Таб.№ | ФИО | Должность | Отдел\n";
            report += "------------------------------------------------\n";

            foreach (var emp in _employees.Where(e => !e.IsArchived))
            {
                report += $"{emp.PersonalNumber,-6} | {emp.LastName} {emp.FirstName} | {emp.PositionName,-20} | {emp.DepartmentName}\n";
            }

            ReportTextBlock.Text = report;
            StatusText.Text = "Сформирован отчет по сотрудникам";
        }

        private void PersonnelMovementReportButton_Click(object sender, RoutedEventArgs e)
        {
            string report = "=== ДВИЖЕНИЕ КАДРОВ ===\n\n";
            report += $"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm}\n\n";
            report += $"Принято: {_employees.Count(e => e.HireDate.HasValue && e.HireDate.Value.Year == DateTime.Now.Year)}\n";
            report += $"Уволено: {_employees.Count(e => e.IsArchived && e.DismissalDate.HasValue && e.DismissalDate.Value.Year == DateTime.Now.Year)}\n";
            report += $"Текущая численность: {_employees.Count(e => !e.IsArchived)}\n";

            ReportTextBlock.Text = report;
            StatusText.Text = "Сформирован отчет по движению кадров";
        }

        private void BirthdaysReportButton_Click(object sender, RoutedEventArgs e)
        {
            string report = "=== ДНИ РОЖДЕНИЯ СОТРУДНИКОВ ===\n\n";
            report += $"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm}\n\n";

            var birthdays = _employees
                .Where(e => !e.IsArchived)
                .OrderBy(e => e.BirthDate.Month)
                .ThenBy(e => e.BirthDate.Day);

            foreach (var emp in birthdays)
            {
                report += $"{emp.LastName} {emp.FirstName} - {emp.BirthDate:dd.MM} ({emp.BirthDate:dd.MM.yyyy})\n";
            }

            ReportTextBlock.Text = report;
            StatusText.Text = "Сформирован отчет по дням рождения";
        }

        private void VacationsReportButton_Click(object sender, RoutedEventArgs e)
        {
            string report = "=== ОТЧЕТ ПО ОТПУСКАМ ===\n\n";
            report += $"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm}\n\n";
            report += "------------------------------------------------\n";
            report += "Сотрудник | Период отпуска | Кол-во дней\n";
            report += "------------------------------------------------\n";

            foreach (var vac in _vacations)
            {
                report += $"{vac.EmployeeFullName,-25} | {vac.StartDate:dd.MM}-{vac.EndDate:dd.MM} | {vac.DaysCount}\n";
            }

            ReportTextBlock.Text = report;
            StatusText.Text = "Сформирован отчет по отпускам";
        }

        private void MilitaryReportButton_Click(object sender, RoutedEventArgs e)
        {
            string report = "=== ВОИНСКИЙ УЧЕТ ===\n\n";
            report += $"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm}\n\n";
            report += "Список военнообязанных:\n";
            report += "------------------------------------------------\n";
            report += "Функция воинского учета будет реализована\n";

            ReportTextBlock.Text = report;
            StatusText.Text = "Сформирован отчет по воинскому учету";
        }
    }
}