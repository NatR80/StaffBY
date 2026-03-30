using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StaffBY.App.ViewModels;
using StaffBY.Domain.Entities;


namespace StaffBY.App.Views.UserControls
{
    public partial class ReportsControl : UserControl
    {
        public event Action<string>? StatusMessageChanged;
        public List<EmployeeViewModel> Employees { get; set; } = new List<EmployeeViewModel>();
        public List<VacationViewModel> Vacations { get; set; } = new List<VacationViewModel>();

        public ReportsControl()
        {
            InitializeComponent();
        }

        public void SetEmployees(List<EmployeeViewModel> employees) => Employees = employees;
        public void SetVacations(List<VacationViewModel> vacations) => Vacations = vacations;

        private void EmployeeList_Click(object sender, RoutedEventArgs e)
        {
            if (!Employees.Any())
            {
                txtReport.Text = "Нет данных о сотрудниках";
                return;
            }

            string report = "СПИСОК СОТРУДНИКОВ\n" + new string('=', 50) + "\n\n";
            foreach (var emp in Employees.Where(e => !e.IsArchived))
            {
                report += $"Табельный номер: {emp.PersonalNumber}\n";
                report += $"ФИО: {emp.FullName}\n";
                report += $"Должность: {emp.PositionName}\n";
                report += $"Отдел: {emp.DepartmentName}\n";
                report += $"Телефон: {emp.Phone}\n";
                report += new string('-', 40) + "\n";
            }
            txtReport.Text = report;
            StatusMessageChanged?.Invoke("Отчет сформирован");
        }

        private void MovementReport_Click(object sender, RoutedEventArgs e) =>
            txtReport.Text = "Движение кадров\n" + new string('=', 30) + "\nОтчет в разработке";

        private void BirthdaysReport_Click(object sender, RoutedEventArgs e)
        {
            var today = DateTime.Now;
            var birthdays = Employees.Where(e => e.BirthDate.Month == today.Month).ToList();

            string report = "ДНИ РОЖДЕНИЯ В ТЕКУЩЕМ МЕСЯЦЕ\n" + new string('=', 40) + "\n\n";
            foreach (var emp in birthdays)
                report += $"{emp.FullName} - {emp.BirthDate:dd.MM.yyyy}\n";

            txtReport.Text = birthdays.Any() ? report : "Нет дней рождения в этом месяце";
        }

        private void VacationsReport_Click(object sender, RoutedEventArgs e)
        {
            string report = "ОТЧЕТ ПО ОТПУСКАМ\n" + new string('=', 30) + "\n\n";
            foreach (var vac in Vacations)
                report += $"{vac.EmployeeFullName}: {vac.StartDate:dd.MM.yyyy} - {vac.EndDate:dd.MM.yyyy} ({vac.DaysCount} дн.)\n";
            txtReport.Text = report;
        }

        private void MilitaryReport_Click(object sender, RoutedEventArgs e) =>
            txtReport.Text = "ВОИНСКИЙ УЧЕТ\n" + new string('=', 30) + "\nОтчет в разработке";
    }
}