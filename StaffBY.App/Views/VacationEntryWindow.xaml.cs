using System;
using System.Windows;
using StaffBY.App.Models;

namespace StaffBY.App.Views
{
    public partial class VacationEntryWindow : Window
    {
        private VacationEntry _period;

        public event Action<int, DateTime, string>? VacationEntrySaved;

        public VacationEntryWindow(VacationEntry period)
        {
            InitializeComponent();
            _period = period;

            txtPeriodInfo.Text = $"{period.PeriodName}";
            txtRemainingDays.Text = period.RemainingDays.ToString();

            // Ограничиваем максимальное количество дней
            txtDaysCount.MaxLength = period.RemainingDays.ToString().Length + 1;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(txtDaysCount.Text, out int days) || days <= 0)
                {
                    MessageBox.Show("Введите корректное количество дней", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (days > _period.RemainingDays)
                {
                    MessageBox.Show($"Количество дней ({days}) превышает остаток ({_period.RemainingDays})", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!dpStartDate.SelectedDate.HasValue)
                {
                    MessageBox.Show("Выберите дату начала отпуска", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string basis = string.IsNullOrWhiteSpace(txtBasis.Text) ? "" : txtBasis.Text;

                VacationEntrySaved?.Invoke(days, dpStartDate.SelectedDate.Value, basis);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
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