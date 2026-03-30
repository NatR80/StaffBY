using System;
using System.Linq;
using System.Windows;
using StaffBY.Domain.Entities;
using StaffBY.App.Views.UserControls;
using StaffBY.App.ViewModels;

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

            StatusText.Text = _currentUser != null
                ? $"Добро пожаловать, {currentUser.Username}!"
                : "Готов к работе";
        }

        private void SubscribeToEvents()
        {
            // Подписка на события EmployeesControl
            if (EmployeesControl != null)
            {
                EmployeesControl.StatusMessageChanged += msg => StatusText.Text = msg;
                EmployeesControl.EmployeeCountChanged += count => StatusText.Text = $"Сотрудников: {count}";
                EmployeesControl.EmployeesChanged += () => LoadData(); // ВАЖНО: обновляем данные при изменении
            }

            // Подписка на события всех контролов
            if (PositionsControl != null)
                PositionsControl.StatusMessageChanged += msg => StatusText.Text = msg;

            if (VacationsControl != null)
                VacationsControl.StatusMessageChanged += msg => StatusText.Text = msg;

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
                // Получаем ВСЕХ сотрудников (включая архивированных)
                var allEmployees = EmployeesControl?.GetAllEmployees() ?? new System.Collections.Generic.List<EmployeeViewModel>();

                // Получаем только активных сотрудников
                var activeEmployees = allEmployees.Where(e => !e.IsArchived).ToList();

                // Передаем ВСЕХ сотрудников в ArchiveControl (нужны уволенные)
                ArchiveControl?.SetEmployees(allEmployees);

                // Передаем активных сотрудников в другие контролы
                TimesheetControl?.SetEmployees(activeEmployees);
                AccrualsControl?.SetEmployees(activeEmployees);
                DocumentsControl?.SetEmployees(activeEmployees);
                ReportsControl?.SetEmployees(activeEmployees);

                // Для отпусков нужны активные сотрудники
                var vacations = VacationsControl?.GetVacations() ?? new System.Collections.Generic.List<VacationViewModel>();
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
    }
}