using System;
using System.Windows;
using System.Windows.Controls;

namespace StaffBY.App.Views
{
    public partial class VacationEditWindow : Window
    {
        private VacationEntry _vacation;
        private bool _isEditMode;

        public event EventHandler<VacationEntry> VacationSaved;

        public VacationEditWindow(VacationEntry vacation = null)
        {
            InitializeComponent();

            if (vacation == null)
            {
                _isEditMode = false;
                _vacation = new VacationEntry();
                Title = "Добавление отпуска";
            }
            else
            {
                _isEditMode = true;
                _vacation = vacation;
                Title = "Редактирование отпуска";
                LoadData();
            }
        }

        private void LoadData()
        {
            // Загружаем данные из существующего отпуска
            cmbVacationType.Text = _vacation.VacationType;
            dpPeriodStart.SelectedDate = ParsePeriodStart(_vacation.Period);
            dpPeriodEnd.SelectedDate = ParsePeriodEnd(_vacation.Period);
            dpStartDate.SelectedDate = _vacation.StartDate;
            dpEndDate.SelectedDate = _vacation.EndDate;
            txtDaysCount.Text = _vacation.DaysCount.ToString();
            txtBasis.Text = _vacation.Basis;
        }

        private DateTime? ParsePeriodStart(string period)
        {
            if (string.IsNullOrEmpty(period)) return null;
            var parts = period.Split('-');
            if (parts.Length > 0 && DateTime.TryParse(parts[0].Trim(), out DateTime start))
                return start;
            return null;
        }

        private DateTime? ParsePeriodEnd(string period)
        {
            if (string.IsNullOrEmpty(period)) return null;
            var parts = period.Split('-');
            if (parts.Length > 1 && DateTime.TryParse(parts[1].Trim(), out DateTime end))
                return end;
            return null;
        }

        private void EndDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateDays();
        }

        private void CalculateDays()
        {
            if (dpStartDate.SelectedDate.HasValue && dpEndDate.SelectedDate.HasValue)
            {
                var start = dpStartDate.SelectedDate.Value;
                var end = dpEndDate.SelectedDate.Value;

                if (end >= start)
                {
                    int days = (end - start).Days + 1;
                    txtDaysCount.Text = days.ToString();
                }
                else
                {
                    txtDaysCount.Text = "0";
                }
            }
            else
            {
                txtDaysCount.Text = "0";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка обязательных полей
            if (cmbVacationType.SelectedItem == null)
            {
                MessageBox.Show("Выберите вид отпуска", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!dpStartDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату начала отпуска", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!dpEndDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату окончания отпуска", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpEndDate.SelectedDate < dpStartDate.SelectedDate)
            {
                MessageBox.Show("Дата окончания не может быть раньше даты начала", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // РАСЧЕТ ДНЕЙ ПРЯМО ЗДЕСЬ
            var start = dpStartDate.SelectedDate.Value;
            var end = dpEndDate.SelectedDate.Value;
            int days = (end - start).Days + 1;

            // Сохраняем данные
            _vacation.VacationType = (cmbVacationType.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (dpPeriodStart.SelectedDate.HasValue && dpPeriodEnd.SelectedDate.HasValue)
            {
                _vacation.Period = $"{dpPeriodStart.SelectedDate.Value:dd.MM.yyyy} - {dpPeriodEnd.SelectedDate.Value:dd.MM.yyyy}";
            }

            _vacation.StartDate = start;
            _vacation.EndDate = end;
            _vacation.DaysCount = days;  // Используем рассчитанные дни
            _vacation.Basis = txtBasis.Text;

            // Генерируем ID для нового отпуска
            if (!_isEditMode)
            {
                _vacation.Id = DateTime.Now.Millisecond;
            }

            VacationSaved?.Invoke(this, _vacation);

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}