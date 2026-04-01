using System;
using System.Linq;
using System.Windows;
using static StaffBY.App.Views.EmployeeEditWindow;

namespace StaffBY.App.Views
{
    public partial class VacationEditWindow : Window
    {
        private VacationEntry _vacation;
        private bool _isEditMode;
        private int _totalDaysPerYear = 24;

        public event Action<VacationEntry> VacationSaved;

        public VacationEditWindow(VacationEntry vacation = null, DateTime? defaultPeriodStart = null)
        {
            InitializeComponent();

            // Получаем общее количество дней из главного окна
            var mainWindow = Application.Current.Windows.OfType<EmployeeEditWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                // Получаем значение из правой колонки
                var totalYearText = mainWindow.GetTotalYearVacation();
                if (int.TryParse(totalYearText, out int totalYear))
                    _totalDaysPerYear = totalYear;
            }

            if (vacation == null)
            {
                _isEditMode = false;
                _vacation = new VacationEntry();
                Title = "Добавление отпуска";

                if (defaultPeriodStart.HasValue)
                {
                    dpPeriodStart.SelectedDate = defaultPeriodStart.Value;
                    dpPeriodEnd.SelectedDate = defaultPeriodStart.Value.AddYears(1).AddDays(-1);
                }
            }
            else
            {
                _isEditMode = true;
                _vacation = vacation;
                Title = "Редактирование отпуска";
                LoadVacationData();
            }

            txtBasicDays.Text = _totalDaysPerYear.ToString();
            txtAdditionalDays.Text = "0"; // будет обновлено позже
        }

        private void LoadVacationData()
        {
            dpPeriodStart.SelectedDate = _vacation.PeriodStart;
            dpPeriodEnd.SelectedDate = _vacation.PeriodEnd;
            txtUsedDays.Text = _vacation.UsedDays.ToString();
            dpStartDate.SelectedDate = _vacation.StartDate;
            txtBasis.Text = _vacation.Basis;
            txtSchedule.Text = _vacation.Schedule;

            if (_vacation.EndDate.HasValue)
                txtEndDate.Text = _vacation.EndDate.Value.ToString("dd.MM.yyyy");
        }

        private void UsedDays_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CalculateEndDate();
        }

        private void StartDate_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CalculateEndDate();
        }

        private void CalculateEndDate()
        {
            if (dpStartDate.SelectedDate.HasValue && int.TryParse(txtUsedDays.Text, out int days) && days > 0)
            {
                var endDate = dpStartDate.SelectedDate.Value.AddDays(days - 1);
                txtEndDate.Text = endDate.ToString("dd.MM.yyyy");
            }
            else
            {
                txtEndDate.Text = "";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!dpPeriodStart.SelectedDate.HasValue || !dpPeriodEnd.SelectedDate.HasValue)
                {
                    MessageBox.Show("Укажите период работы", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(txtUsedDays.Text, out int usedDays) || usedDays < 0)
                {
                    MessageBox.Show("Введите корректное количество дней", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _vacation.PeriodStart = dpPeriodStart.SelectedDate.Value;
                _vacation.PeriodEnd = dpPeriodEnd.SelectedDate.Value;
                _vacation.UsedDays = usedDays;
                _vacation.StartDate = dpStartDate.SelectedDate;

                if (dpStartDate.SelectedDate.HasValue && usedDays > 0)
                    _vacation.EndDate = dpStartDate.SelectedDate.Value.AddDays(usedDays - 1);
                else
                    _vacation.EndDate = null;

                _vacation.Basis = txtBasis.Text;
                _vacation.Schedule = txtSchedule.Text;

                if (!_isEditMode)
                {
                    _vacation.Id = DateTime.Now.GetHashCode();
                    _vacation.BasicDays = 24;
                    _vacation.AdditionalDays = 0; // будет обновлено из главной формы
                }

                VacationSaved?.Invoke(_vacation);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}