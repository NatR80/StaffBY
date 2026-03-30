using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StaffBY.App.ViewModels;

namespace StaffBY.App.Views.UserControls
{
    /// <summary>
    /// Логика взаимодействия для EmployeesControl.xaml
    /// Контрол для управления списком сотрудников
    /// </summary>
    public partial class EmployeesControl : UserControl
    {
        // ПОЛЯ И СВОЙСТВА
        private List<EmployeeViewModel> _employees = new List<EmployeeViewModel>();

        // СОБЫТИЯ
        public event Action<string>? StatusMessageChanged;   // Для статусной строки
        public event Action<int>? EmployeeCountChanged;     // Для счетчика сотрудников
        public event Action? EmployeesChanged;               // НОВОЕ СОБЫТИЕ: при изменении списка сотрудников

        

        // КОНСТРУКТОР
        public EmployeesControl()
        {
            InitializeComponent();
            LoadDemoData();
            RefreshGrid();
        }

        /// <summary>
        /// Загружает демо-данные (временные, пока нет БД)
        /// </summary>
        public void LoadDemoData()
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
                    Phone = "+375291234567",
                    Salary = 5000,
                    IsArchived = false
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
                    Phone = "+375292345678",
                    Salary = 3500,
                    IsArchived = false
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
                    Phone = "+375293456789",
                    Salary = 2500,
                    IsArchived = false
                }
            };

            // Вызываем событие для счетчика
            EmployeeCountChanged?.Invoke(_employees.Count);
        }

        /// <summary>
        /// Возвращает список активных (не архивированных) сотрудников
        /// </summary>
        public List<EmployeeViewModel> GetEmployees()
        {
            return _employees.Where(e => !e.IsArchived).ToList();
        }

        /// <summary>
        /// Возвращает ВСЕХ сотрудников (включая архивированных)
        /// </summary>
        public List<EmployeeViewModel> GetAllEmployees()
        {
            return _employees.ToList();
        }

        /// <summary>
        /// Обновляет отображение DataGrid
        /// </summary>
        private void RefreshGrid()
        {
            // Показываем только активных сотрудников
            var activeEmployees = _employees.Where(e => !e.IsArchived).ToList();

            // Находим DataGrid (предполагаем имя EmployeesDataGrid)
            var dataGrid = FindName("EmployeesDataGrid") as DataGrid;
            if (dataGrid != null)
            {
                dataGrid.ItemsSource = activeEmployees;
            }

            // Вызываем события для обновления других контролов
            EmployeeCountChanged?.Invoke(activeEmployees.Count);
            StatusMessageChanged?.Invoke($"Записей: {activeEmployees.Count}");

            // ВАЖНО: Уведомляем другие контролы об изменении списка сотрудников
            EmployeesChanged?.Invoke();
        }

        /// <summary>
        /// Поиск сотрудников по тексту
        /// </summary>
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();
            var dataGrid = FindName("EmployeesDataGrid") as DataGrid;

            if (dataGrid == null) return;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                RefreshGrid();
            }
            else
            {
                var filtered = _employees
                    .Where(e => !e.IsArchived && (
                        e.LastName.ToLower().Contains(searchText) ||
                        e.FirstName.ToLower().Contains(searchText) ||
                        (e.Patronymic?.ToLower().Contains(searchText) ?? false) ||
                        e.PersonalNumber.Contains(searchText)))
                    .ToList();

                dataGrid.ItemsSource = filtered;
                StatusMessageChanged?.Invoke($"Найдено: {filtered.Count} записей");
            }
        }

        /// <summary>
        /// Кнопка поиска
        /// </summary>
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox_TextChanged(sender, null);
        }

        /// <summary>
        /// Кнопка экспорта в Excel
        /// </summary>
        private void ExportToExcelButton_Click(object sender, RoutedEventArgs e)
        {
            StatusMessageChanged?.Invoke("Экспорт в Excel...");
            // Здесь будет логика экспорта
        }

        /// <summary>
        /// Кнопка добавления нового сотрудника
        /// </summary>
        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EmployeeEditWindow();
            editWindow.EmployeeSaved += (s, employee) =>
            {
                // Генерируем новый ID
                employee.Id = _employees.Count > 0 ? _employees.Max(x => x.Id) + 1 : 1;
                _employees.Add(employee);
                RefreshGrid();
                StatusMessageChanged?.Invoke($"Сотрудник {employee.LastName} {employee.FirstName} добавлен");
            };
            editWindow.Owner = Window.GetWindow(this);
            editWindow.ShowDialog();
        }

        /// <summary>
        /// Двойной клик по строке - редактирование сотрудника
        /// </summary>
        private void EmployeesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid?.SelectedItem is EmployeeViewModel selected)
            {
                var editWindow = new EmployeeEditWindow(selected);
                editWindow.EmployeeSaved += (s, employee) =>
                {
                    var index = _employees.FindIndex(x => x.Id == employee.Id);
                    if (index >= 0)
                    {
                        _employees[index] = employee;
                        RefreshGrid();
                        StatusMessageChanged?.Invoke($"Сотрудник {employee.LastName} {employee.FirstName} обновлен");
                    }
                };
                editWindow.Owner = Window.GetWindow(this);
                editWindow.ShowDialog();
            }
        }

        /// <summary>
        /// Кнопка увольнения сотрудника
        /// </summary>
        private void FireEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            var dataGrid = FindName("EmployeesDataGrid") as DataGrid;
            if (dataGrid?.SelectedItem is EmployeeViewModel selected)
            {
                if (MessageBox.Show($"Уволить сотрудника {selected.LastName} {selected.FirstName}?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    selected.IsArchived = true;
                    RefreshGrid();
                    StatusMessageChanged?.Invoke($"Сотрудник {selected.LastName} {selected.FirstName} уволен и перемещен в архив");
                }
            }
            else
            {
                StatusMessageChanged?.Invoke("Выберите сотрудника для увольнения");
            }
        }
    }
}