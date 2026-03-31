using System;
using System.Windows;
using StaffBY.App.Views;
using static StaffBY.App.Views.EmployeeEditWindow;  // Добавьте эту строку



namespace StaffBY.App.Views
{
    public partial class VacationEditWindow : Window
    {
        private VacationEntry _vacation;
        private bool _isEditMode;

        public event Action<VacationEntry> VacationSaved;

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
                LoadVacationData();
            }

            // Подписываемся на изменение дат для автоматического расчета дней
            dpStartDate.SelectedDateChanged += Dates_SelectedDateChanged;
            dpEndDate.SelectedDateChanged += Dates_SelectedDateChanged;
        }

        private void LoadVacationData()
        {
            // Вид отпуска (если нужно)
            dpPeriodStart.SelectedDate = _vacation.PeriodStart;
            dpPeriodEnd.SelectedDate = _vacation.PeriodEnd;
            dpStartDate.SelectedDate = _vacation.StartDate;
            dpEndDate.SelectedDate = _vacation.EndDate;
            txtDaysCount.Text = _vacation.UsedDays.ToString();
            txtBasis.Text = _vacation.Basis;
        }

        private void Dates_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
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
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!dpPeriodStart.SelectedDate.HasValue || !dpPeriodEnd.SelectedDate.HasValue)
                {
                    MessageBox.Show("Укажите рабочий период (за какой год предоставляется отпуск)", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!dpStartDate.SelectedDate.HasValue || !dpEndDate.SelectedDate.HasValue)
                {
                    MessageBox.Show("Укажите даты начала и окончания отпуска", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _vacation.PeriodStart = dpPeriodStart.SelectedDate.Value;
                _vacation.PeriodEnd = dpPeriodEnd.SelectedDate.Value;
                _vacation.StartDate = dpStartDate.SelectedDate.Value;
                _vacation.EndDate = dpEndDate.SelectedDate.Value;

                if (int.TryParse(txtDaysCount.Text, out int days))
                    _vacation.UsedDays = days;
                else
                    _vacation.UsedDays = 0;

                _vacation.Basis = txtBasis.Text;

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