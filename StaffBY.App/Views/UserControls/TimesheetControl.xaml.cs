using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StaffBY.App.ViewModels;

namespace StaffBY.App.Views.UserControls
{
    public partial class TimesheetControl : UserControl
    {
        private List<TimesheetViewModel> _timesheets = new List<TimesheetViewModel>();
        public event Action<string>? StatusMessageChanged;
        public List<EmployeeViewModel> Employees { get; set; } = new List<EmployeeViewModel>();

        public TimesheetControl()
        {
            InitializeComponent();
            SetDefaultDates();
        }

        public void SetEmployees(List<EmployeeViewModel> employees)
        {
            Employees = employees;
        }

        private void SetDefaultDates()
        {
            var today = DateTime.Now;
            var firstDay = new DateTime(today.Year, today.Month, 1);
            dpStart.SelectedDate = firstDay;
            dpEnd.SelectedDate = firstDay.AddMonths(1).AddDays(-1);
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Employees.Any())
            {
                StatusMessageChanged?.Invoke("Нет сотрудников для формирования табеля");
                return;
            }

            var start = dpStart.SelectedDate ?? DateTime.Now;
            var end = dpEnd.SelectedDate ?? DateTime.Now;
            var random = new Random();
            int totalDays = (end - start).Days + 1;

            _timesheets = Employees.Select(emp => new TimesheetViewModel
            {
                EmployeeId = emp.Id,
                PersonalNumber = emp.PersonalNumber,
                FullName = emp.FullName,
                WorkedDays = random.Next(15, totalDays),
                SickDays = random.Next(0, 5),
                VacationDays = random.Next(0, 10),
                AbsentDays = random.Next(0, 3)
            }).ToList();

            TimesheetDataGrid.ItemsSource = _timesheets;
            StatusMessageChanged?.Invoke($"Табель сформирован за {start:dd.MM.yyyy} - {end:dd.MM.yyyy}");
        }
    }
}