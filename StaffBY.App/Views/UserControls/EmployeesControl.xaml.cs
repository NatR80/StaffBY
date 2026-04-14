using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StaffBY.App.ViewModels;
using StaffBY.App.Views;
using StaffBY.App.Views.UserControls.EmployeeCard;


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
            // Получаем список должностей из PositionsControl
            var positionsControl = FindPositionsControl();
            if (positionsControl == null)
            {
                StatusMessageChanged?.Invoke("Ошибка: не найден контрол штатного расписания");
                return;
            }

            var positions = positionsControl.GetPositions();

            var employeeCard = new EmployeeCardMain();
            var newEmployee = new EmployeeViewModel();

            // Открываем карточку для нового сотрудника
            employeeCard.LoadData(newEmployee, positions);
            employeeCard.EmployeeSaved += (s, savedEmployee) =>
            {
                savedEmployee.Id = _employees.Count > 0 ? _employees.Max(x => x.Id) + 1 : 1;
                _employees.Add(savedEmployee);
                RefreshGrid();
                StatusMessageChanged?.Invoke($"Сотрудник {savedEmployee.LastName} {savedEmployee.FirstName} добавлен");
            };

            var window = new Window
            {
                Title = "Новый сотрудник",
                Content = employeeCard,
                Width = 1100,
                Height = 800,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this)
            };
            window.ShowDialog();
        }


        /// <summary>
        /// Двойной клик по строке - редактирование сотрудника
        /// </summary>
        private void EmployeesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid?.SelectedItem is EmployeeViewModel selected)
            {
                // Получаем список должностей из PositionsControl
                var positionsControl = FindPositionsControl();
                if (positionsControl == null)
                {
                    StatusMessageChanged?.Invoke("Ошибка: не найден контрол штатного расписания");
                    return;
                }

                var positions = positionsControl.GetPositions();

                var employeeCard = new EmployeeCardMain();
                employeeCard.LoadData(selected, positions);
                employeeCard.EmployeeSaved += (s, savedEmployee) =>
                {
                    var index = _employees.FindIndex(x => x.Id == savedEmployee.Id);
                    if (index >= 0)
                    {
                        _employees[index] = savedEmployee;
                        RefreshGrid();
                        StatusMessageChanged?.Invoke($"Сотрудник {savedEmployee.LastName} {savedEmployee.FirstName} обновлен");
                    }
                };

                var window = new Window
                {
                    Title = $"Редактирование сотрудника: {selected.FullName}",
                    Content = employeeCard,
                    Width = 1100,
                    Height = 800,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = Window.GetWindow(this)
                };
                window.ShowDialog();
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

        /// <summary>
        /// Находит контрол PositionsControl на главном окне
        /// </summary>
        private PositionsControl? FindPositionsControl()
        {
            var parent = Window.GetWindow(this);
            if (parent is MainWindow mainWindow)
            {
                // Ищем поле PositionsControl в MainWindow
                var field = mainWindow.GetType().GetField("PositionsControl",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    return field.GetValue(mainWindow) as PositionsControl;
                }

                // Альтернативный способ: ищем по имени контрола
                var control = mainWindow.FindName("PositionsControl");
                if (control is PositionsControl positionsControl)
                {
                    return positionsControl;
                }
            }
            return null;
        }

        //public void SetReadOnly(bool isReadOnly)
        //{
        //    AddEmployeeButton.IsEnabled = !isReadOnly;
        //    FireEmployeeButton.IsEnabled = !isReadOnly;
        //    // Запретить редактирование в DataGrid
        //    if (EmployeesDataGrid != null)
        //        EmployeesDataGrid.IsReadOnly = isReadOnly;
        //}
    }
}