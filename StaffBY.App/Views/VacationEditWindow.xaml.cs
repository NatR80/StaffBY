using System;
using System.Windows;
using System.Windows.Controls;
using StaffBY.App.Models;

namespace StaffBY.App.Views
{
    public partial class VacationEditWindow : Window
    {
        private VacationEntry _vacation;
        private bool _isEditMode;
        private int _basicDays = 24;
        private int _additionalDays = 0;

        public event Action<VacationEntry>? VacationSaved;

        public VacationEditWindow(VacationEntry? vacation = null, DateTime? defaultPeriodStart = null, int additionalDays = 0)
        {
            InitializeComponent();

            _additionalDays = additionalDays;

            if (vacation == null)
            {
                _isEditMode = false;
                _vacation = new VacationEntry();
                Title = "➕ Добавление периода отпуска";

                if (defaultPeriodStart.HasValue)
                {
                    dpPeriodStart.SelectedDate = defaultPeriodStart.Value;
                    // Окончание периода = начало + 1 год - 1 день
                    dpPeriodEnd.SelectedDate = defaultPeriodStart.Value.AddYears(1).AddDays(-1);
                    // Предоставляется отпуск с = начало периода + 6 месяцев
                    dpAvailableFrom.SelectedDate = defaultPeriodStart.Value.AddMonths(6);
                }
            }
            else
            {
                _isEditMode = true;
                _vacation = vacation;
                Title = "✏️ Редактирование периода отпуска";
                LoadVacationData();
            }

            txtBasicDays.Text = _basicDays.ToString();
            txtAdditionalDays.Text = _additionalDays.ToString();
            UpdateTotalDays();
        }

        private void LoadVacationData()
        {
            dpPeriodStart.SelectedDate = _vacation.PeriodStart;
            dpPeriodEnd.SelectedDate = _vacation.PeriodEnd;
            dpAvailableFrom.SelectedDate = _vacation.AvailableFrom;
            txtSchedule.Text = _vacation.Schedule;

            _basicDays = _vacation.BasicDays;
            _additionalDays = _vacation.AdditionalDays;
            txtBasicDays.Text = _basicDays.ToString();
            txtAdditionalDays.Text = _additionalDays.ToString();
            UpdateTotalDays();
        }

        private void PeriodStart_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (dpPeriodStart.SelectedDate.HasValue)
            {
                // Автоматически рассчитываем окончание периода
                dpPeriodEnd.SelectedDate = dpPeriodStart.SelectedDate.Value.AddYears(1).AddDays(-1);
                // Автоматически рассчитываем "Предоставляется отпуск с" = начало + 6 месяцев
                dpAvailableFrom.SelectedDate = dpPeriodStart.SelectedDate.Value.AddMonths(6);
            }
        }

        private void UpdateTotalDays()
        {
            int total = _basicDays + _additionalDays;
            txtTotalDays.Text = total.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!dpPeriodStart.SelectedDate.HasValue)
                {
                    MessageBox.Show("Укажите начало периода работы", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    dpPeriodStart.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtSchedule.Text))
                {
                    var result = MessageBox.Show("График отпуска не заполнен. Продолжить?", "Предупреждение",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes)
                    {
                        txtSchedule.Focus();
                        return;
                    }
                }

                _vacation.PeriodStart = dpPeriodStart.SelectedDate.Value;
                _vacation.PeriodEnd = dpPeriodEnd.SelectedDate.Value;
                _vacation.BasicDays = _basicDays;
                _vacation.AdditionalDays = _additionalDays;
                _vacation.AvailableFrom = dpAvailableFrom.SelectedDate;
                _vacation.Schedule = txtSchedule.Text;
                _vacation.UsedDays = 0; // Использовано дней заполняется позже в таблице
                _vacation.StartDate = null;
                _vacation.EndDate = null;
                _vacation.Basis = string.Empty;

                if (!_isEditMode)
                {
                    _vacation.Id = DateTime.Now.GetHashCode();
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