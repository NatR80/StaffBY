using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using StaffBY.App.ViewModels;

namespace StaffBY.App.Views.UserControls
{
    /// <summary>
    /// Логика взаимодействия для ArchiveControl.xaml
    /// Контрол для отображения архивированных (уволенных) сотрудников
    /// </summary>
    public partial class ArchiveControl : UserControl
    {
        // Событие для отправки сообщений в статусную строку главного окна
        public event Action<string>? StatusMessageChanged;

        // Полный список всех сотрудников (полученный из EmployeesControl)
        private List<EmployeeViewModel> _allEmployees = new List<EmployeeViewModel>();

        // Список архивированных сотрудников (кеш для быстрого доступа)
        private List<EmployeeViewModel> _archivedEmployees = new List<EmployeeViewModel>();

        public ArchiveControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Устанавливает список всех сотрудников и обновляет архив
        /// </summary>
        /// <param name="employees">Полный список сотрудников из EmployeesControl</param>
        public void SetEmployees(List<EmployeeViewModel> employees)
        {
            _allEmployees = employees ?? new List<EmployeeViewModel>();

            // Обновляем список архивированных
            RefreshArchiveList();

            // Отправляем сообщение в статусную строку
            StatusMessageChanged?.Invoke($"Загружено {_allEmployees.Count} сотрудников, в архиве: {_archivedEmployees.Count}");
        }

        /// <summary>
        /// Обновляет список архивированных сотрудников
        /// </summary>
        private void RefreshArchiveList()
        {
            // Отбираем только архивированных сотрудников
            _archivedEmployees = _allEmployees
                .Where(e => e.IsArchived)
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ToList();

            // Обновляем отображение в DataGrid
            RefreshGrid();
        }

        /// <summary>
        /// Обновляет отображение DataGrid (вызывается при изменении данных)
        /// </summary>
        public void RefreshGrid()
        {
            // Находим DataGrid в визуальном дереве или используем поле
            // Предполагаем, что в XAML есть DataGrid с именем ArchiveDataGrid
            var dataGrid = FindName("ArchiveDataGrid") as DataGrid;
            if (dataGrid != null)
            {
                dataGrid.ItemsSource = _archivedEmployees;
            }

            // Отправляем сообщение о количестве архивированных
            StatusMessageChanged?.Invoke($"В архиве: {_archivedEmployees.Count} сотрудников");
        }

        /// <summary>
        /// Восстанавливает сотрудника из архива
        /// </summary>
        /// <param name="employeeId">ID сотрудника для восстановления</param>
        public void RestoreEmployee(int employeeId)
        {
            var employee = _archivedEmployees.FirstOrDefault(e => e.Id == employeeId);
            if (employee != null)
            {
                employee.IsArchived = false;
                RefreshArchiveList();
                StatusMessageChanged?.Invoke($"Сотрудник {employee.LastName} {employee.FirstName} восстановлен из архива");
            }
        }

        /// <summary>
        /// Получает список всех архивированных сотрудников
        /// </summary>
        public List<EmployeeViewModel> GetArchivedEmployees()
        {
            return _archivedEmployees.ToList();
        }

        /// <summary>
        /// Получает список всех сотрудников (включая архивированных)
        /// </summary>
        public List<EmployeeViewModel> GetAllEmployees()
        {
            return _allEmployees.ToList();
        }

        /// <summary>
        /// Обработчик двойного клика по строке - восстановление сотрудника
        /// </summary>
        private void ArchiveDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Находим DataGrid
            var dataGrid = sender as DataGrid;
            if (dataGrid?.SelectedItem is EmployeeViewModel selected)
            {
                RestoreEmployee(selected.Id);
            }
        }

        /// <summary>
        /// Обработчик кнопки "Восстановить"
        /// </summary>
        private void RestoreButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Находим DataGrid
            var dataGrid = FindName("ArchiveDataGrid") as DataGrid;
            if (dataGrid?.SelectedItem is EmployeeViewModel selected)
            {
                RestoreEmployee(selected.Id);
            }
            else
            {
                StatusMessageChanged?.Invoke("Выберите сотрудника для восстановления");
            }
        }
    }
}