using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StaffBY.App.ViewModels;

namespace StaffBY.App.Views.UserControls
{
    /// <summary>
    /// Логика взаимодействия для AccrualsControl.xaml
    /// Контрол для начисления заработной платы
    /// </summary>
    public partial class AccrualsControl : UserControl
    {
        // Событие для отправки сообщений в статусную строку главного окна
        public event Action<string>? StatusMessageChanged;

        // Список всех сотрудников (полученный из EmployeesControl)
        private List<EmployeeViewModel> _employees = new List<EmployeeViewModel>();

        // Данные о начислениях за текущий месяц
        private List<AccrualViewModel> _accruals = new List<AccrualViewModel>();

        public AccrualsControl()
        {
            InitializeComponent();
            LoadDemoAccruals(); // Загружаем демо-данные для наглядности
        }

        /// <summary>
        /// Устанавливает список сотрудников из EmployeesControl
        /// </summary>
        /// <param name="employees">Список сотрудников</param>
        public void SetEmployees(List<EmployeeViewModel> employees)
        {
            _employees = employees ?? new List<EmployeeViewModel>();

            // Отправляем сообщение в статусную строку
            StatusMessageChanged?.Invoke($"Загружено {_employees.Count} сотрудников для начисления ЗП");

            // Обновляем отображение начислений
            RefreshAccrualsDisplay();
        }

        /// <summary>
        /// Загружает демо-данные для начисления ЗП
        /// </summary>
        private void LoadDemoAccruals()
        {
            _accruals = new List<AccrualViewModel>();
        }

        /// <summary>
        /// Обновляет отображение начислений в DataGrid
        /// </summary>
        private void RefreshAccrualsDisplay()
        {
            if (_employees == null || _employees.Count == 0)
            {
                StatusMessageChanged?.Invoke("Нет сотрудников для расчета начислений");
                return;
            }

            // Создаем данные для отображения (только активные сотрудники)
            var displayData = _employees
                .Where(e => !e.IsArchived)
                .Select(e => new
                {
                    e.Id,
                    e.PersonalNumber,
                    FullName = $"{e.LastName} {e.FirstName} {e.Patronymic}",
                    e.PositionName,
                    e.DepartmentName,
                    e.Salary,
                    // Пример расчета начислений (можно доработать)
                    Bonus = 0m,
                    Total = e.Salary
                }).ToList();

            // Привязываем данные к DataGrid (предполагается, что в XAML есть DataGrid с именем AccrualsDataGrid)
            // Если DataGrid нет, нужно создать. Пока просто отправляем сообщение
            StatusMessageChanged?.Invoke($"Расчет начислений для {displayData.Count} сотрудников");
        }

        /// <summary>
        /// Рассчитывает начисления за указанный месяц и год
        /// </summary>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        public void CalculateMonthAccruals(int year, int month)
        {
            if (_employees == null || _employees.Count == 0)
            {
                StatusMessageChanged?.Invoke("Нет данных сотрудников для расчета");
                return;
            }

            var activeEmployees = _employees.Where(e => !e.IsArchived).ToList();
            StatusMessageChanged?.Invoke($"Расчет начислений за {month:D2}.{year} для {activeEmployees.Count} сотрудников");

            // Здесь можно добавить логику расчета:
            // 1. Получить табель рабочего времени
            // 2. Рассчитать оклад пропорционально отработанным дням
            // 3. Рассчитать премии и надбавки
            // 4. Сохранить результаты в _accruals

            RefreshAccrualsDisplay();
        }

        /// <summary>
        /// Возвращает общую сумму начислений за текущий период
        /// </summary>
        
        public decimal GetTotalAccruals()
        {
            if (_employees == null) return 0;
            return _employees.Where(e => !e.IsArchived).Sum(e => e.Salary ?? 0);
            //                                      ^^^^ добавляем ?? 0 - если Salary null, то берем 0
        }

        /// <summary>
        /// Возвращает список начислений для отчетов
        /// </summary>
        public List<AccrualViewModel> GetAccruals()
        {
            return _accruals;
        }

        /// <summary>
        /// Обработчик кнопки "Рассчитать" (если есть в XAML)
        /// </summary>
        private void CalculateButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var currentDate = DateTime.Now;
            CalculateMonthAccruals(currentDate.Year, currentDate.Month);
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            StatusMessageChanged?.Invoke("Экспорт начислений в Excel...");
            MessageBox.Show("Функция экспорта в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    /// <summary>
    /// Модель представления для начислений
    /// </summary>
    public class AccrualViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Salary { get; set; }      // Оклад
        public decimal Bonus { get; set; }       // Премия
        public decimal Total { get; set; }       // Итого
        public DateTime CalculationDate { get; set; } = DateTime.Now;
    }
}