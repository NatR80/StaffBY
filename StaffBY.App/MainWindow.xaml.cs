using System;
using System.Linq;
using System.Windows;
using StaffBY.Domain.Entities;
using StaffBY.App.Views.UserControls;
using StaffBY.App.ViewModels;
using System.Collections.Generic;
using StaffBY.App.Models;
using StaffBY.Domain.Enums;

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


            // ВАЖНО: вызываем разграничение доступа по роли
            SetAccessByRole(currentUser);

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

        private void SetAccessByRole(User user)
        {
            if (user == null) return;

            // Сначала показываем все вкладки
            SetAllTabsVisibility(Visibility.Visible);

            if (user.Role == UserRole.Admin) // 0 - Admin
            {
                // Админ видит всё
                return;
            }
            else if (user.Role == UserRole.HR) // 1 - HR (Кадровик)
            {
                // Кадровик НЕ видит: Начисление ЗП и Выплата ЗП
                if (AccrualsControl != null)
                    AccrualsControl.Visibility = Visibility.Collapsed;
                if (PaymentControl != null)
                    PaymentControl.Visibility = Visibility.Collapsed;
            }
            else if (user.Role == UserRole.Economist) // 2 - Economist (Экономист)
            {
                // Экономист НЕ видит только: Выплата ЗП
                if (PaymentControl != null)
                    PaymentControl.Visibility = Visibility.Collapsed;
            }
            else if (user.Role == UserRole.Accountant) // 3 - Accountant (Бухгалтер)
            {
                // Бухгалтер видит ТОЛЬКО: Организация, Сотрудники, Начисление ЗП, Выплата ЗП
                // Скрываем всё, кроме этих 4 вкладок
                if (PositionsControl != null)
                    PositionsControl.Visibility = Visibility.Collapsed;
                if (VacationsAllControl != null)
                    VacationsAllControl.Visibility = Visibility.Collapsed;
                if (TimesheetControl != null)
                    TimesheetControl.Visibility = Visibility.Collapsed;
                if (DocumentsControl != null)
                    DocumentsControl.Visibility = Visibility.Collapsed;
                if (ReportsControl != null)
                    ReportsControl.Visibility = Visibility.Collapsed;
                if (ArchiveControl != null)
                    ArchiveControl.Visibility = Visibility.Collapsed;

                // Эти вкладки должны быть видны (убедимся)
                if (OrganizationControl != null)
                    OrganizationControl.Visibility = Visibility.Visible;
                if (EmployeesControl != null)
                    EmployeesControl.Visibility = Visibility.Visible;
                if (AccrualsControl != null)
                    AccrualsControl.Visibility = Visibility.Visible;
                if (PaymentControl != null)
                    PaymentControl.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Устанавливает видимость для всех вкладок
        /// </summary>
        private void SetAllTabsVisibility(Visibility visibility)
        {
            if (OrganizationControl != null)
                OrganizationControl.Visibility = visibility;
            if (EmployeesControl != null)
                EmployeesControl.Visibility = visibility;
            if (PositionsControl != null)
                PositionsControl.Visibility = visibility;
            if (VacationsAllControl != null)
                VacationsAllControl.Visibility = visibility;
            if (TimesheetControl != null)
                TimesheetControl.Visibility = visibility;
            if (AccrualsControl != null)
                AccrualsControl.Visibility = visibility;
            if (PaymentControl != null)
                PaymentControl.Visibility = visibility;
            if (DocumentsControl != null)
                DocumentsControl.Visibility = visibility;
            if (ReportsControl != null)
                ReportsControl.Visibility = visibility;
            if (ArchiveControl != null)
                ArchiveControl.Visibility = visibility;
        }

    }
}