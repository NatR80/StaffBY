using System;
using System.Windows;
using System.Windows.Input;
using StaffBY.App.Models;

namespace StaffBY.App.Views
{
    public partial class UsedDaysInputWindow : Window
    {
        private VacationEntry _vacation;

        public event Action<int, DateTime?, string>? UsedDaysSaved;

        public UsedDaysInputWindow(VacationEntry vacation)
        {
            InitializeComponent();
            _vacation = vacation;

            // Отображаем информацию о периоде
            txtPeriodInfo.Text = $"{_vacation.PeriodName} (остаток: {_vacation.RemainingDays} дней)";
            txtRemainingDays.Text = _vacation.RemainingDays.ToString();

            // Устанавливаем максимальное значение - остаток дней
            // (проверка будет в коде)
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Только цифры
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка количества дней
                if (!int.TryParse(txtDaysCount.Text, out int days) || days <= 0)
                {
                    MessageBox.Show("Введите корректное количество дней (целое положительное число)", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtDaysCount.Focus();
                    return;
                }

                // Проверка, что дни не превышают остаток
                if (days > _vacation.RemainingDays)
                {
                    MessageBox.Show($"Количество дней ({days}) превышает остаток отпуска ({_vacation.RemainingDays} дней).\n\n" +
                        "Пожалуйста, введите корректное количество дней.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtDaysCount.Focus();
                    return;
                }

                // Проверка даты начала
                if (!dpStartDate.SelectedDate.HasValue)
                {
                    MessageBox.Show("Выберите дату начала отпуска", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    dpStartDate.Focus();
                    return;
                }

                // Проверка основания
                if (string.IsNullOrWhiteSpace(txtBasis.Text))
                {
                    var result = MessageBox.Show("Основание (приказ) не заполнено. Продолжить?", "Предупреждение",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes)
                    {
                        txtBasis.Focus();
                        return;
                    }
                }

                // Проверка правила "одна часть не менее 14 дней" (предупреждение будет в основном окне)
                UsedDaysSaved?.Invoke(days, dpStartDate.SelectedDate, txtBasis.Text);
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