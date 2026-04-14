using System;
using System.Linq;
using System.Windows;
using StaffBY.Domain.Entities;
using StaffBY.App.Views.UserControls;
using StaffBY.App.ViewModels;
using System.Collections.Generic;
using StaffBY.App.Models;

namespace StaffBY.App.Views
{
    public partial class MainWindow : Window
    {
        private User _currentUser;

        public MainWindow(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;

            if (currentUser != null)
            {
                UserInfoText.Text = $"Пользователь: {currentUser.Username}";
            }

            SubscribeToEvents();
            LoadData();

            //// ВАЖНО: вызываем разграничение доступа по роли
            //SetAccessByRole(currentUser);

            StatusText.Text = _currentUser != null
                ? $"Добро пожаловать, {currentUser.Username}!"
                : "Готов к работе";
        }

        private void SubscribeToEvents()
        {
            if (EmployeesControl != null)
            {
                EmployeesControl.StatusMessageChanged += msg => StatusText.Text = msg;
                EmployeesControl.EmployeeCountChanged += count => StatusText.Text = $"Сотрудников: {count}";
                EmployeesControl.EmployeesChanged += () => LoadData();
            }

            if (PositionsControl != null)
                PositionsControl.StatusMessageChanged += msg => StatusText.Text = msg;

            if (VacationsAllControl != null)
                VacationsAllControl.StatusMessageChanged += msg => StatusText.Text = msg;

            if (TimesheetControl != null)
                TimesheetControl.StatusMessageChanged += msg => StatusText.Text = msg;

            if (AccrualsControl != null)
                AccrualsControl.StatusMessageChanged += msg => StatusText.Text = msg;

            if (PaymentControl != null)
                PaymentControl.StatusMessageChanged += msg => StatusText.Text = msg;

            if (DocumentsControl != null)
                DocumentsControl.StatusMessageChanged += msg => StatusText.Text = msg;

            if (ReportsControl != null)
                ReportsControl.StatusMessageChanged += msg => StatusText.Text = msg;

            if (ArchiveControl != null)
                ArchiveControl.StatusMessageChanged += msg => StatusText.Text = msg;

            if (OrganizationControl != null)
                OrganizationControl.StatusMessageChanged += msg => StatusText.Text = msg;
        }

        private void LoadData()
        {
            try
            {
                var allEmployees = EmployeesControl?.GetAllEmployees() ?? new List<EmployeeViewModel>();
                var activeEmployees = allEmployees.Where(e => !e.IsArchived).ToList();

                ArchiveControl?.SetEmployees(allEmployees);
                TimesheetControl?.SetEmployees(activeEmployees);
                AccrualsControl?.SetEmployees(activeEmployees);
                DocumentsControl?.SetEmployees(activeEmployees);
                ReportsControl?.SetEmployees(activeEmployees);

                var vacations = VacationsAllControl?.GetVacations() ?? new List<VacationViewModel>();
                ReportsControl?.SetVacations(vacations);

                StatusText.Text = $"Данные загружены. Сотрудников: {activeEmployees.Count}, в архиве: {allEmployees.Count - activeEmployees.Count}";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Ошибка загрузки данных: {ex.Message}";
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите выйти?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.Close();
                var authWindow = new AuthWindow();
                authWindow.Show();
            }
        }

        //private void SetAccessByRole(User user)
        //{
        //    if (user == null) return;

        //    // Скрываем админскую вкладку по умолчанию
        //    TabAdmin.Visibility = Visibility.Collapsed;

        //    switch (user.Role)
        //    {
        //        case 0: // Admin
        //            TabAdmin.Visibility = Visibility.Visible;
        //            break;

        //        case 1: // HR (Кадровик) - штатное расписание только просмотр
        //            if (PositionsControl != null)
        //                PositionsControl.SetReadOnly(true);
        //            break;

        //        case 2: // Economist (Экономист) - сотрудники только просмотр
        //            if (EmployeesControl != null)
        //                EmployeesControl.SetReadOnly(true);
        //            break;

        //        case 3: // Accountant (Бухгалтер) - сотрудники просмотр, штатное расписание скрыто
        //            if (EmployeesControl != null)
        //                EmployeesControl.SetReadOnly(true);
        //            TabPositions.Visibility = Visibility.Collapsed;
        //            break;
        //    }
        //}
    }
}