using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StaffBY.App.ViewModels;
using StaffBY.App.Models;
using StaffBY.App.Views;

namespace StaffBY.App.Views.UserControls.EmployeeCard
{
    public partial class VacationsControl : UserControl
    {
        private List<VacationEntry> _vacations = new();
        private List<VacationEntry> _vacationsArchive = new();
        private EmployeeViewModel? _employee;

        public VacationsControl()
        {
            InitializeComponent();
        }

        public void LoadData(EmployeeViewModel employee)
        {
            _employee = employee ?? throw new ArgumentNullException(nameof(employee));
            _vacations = new List<VacationEntry>();
            _vacationsArchive = new List<VacationEntry>();

            txtContractDays.Text = employee.ContractVacationDays.ToString();
            txtHarmfulDays.Text = employee.HarmfulVacationDays.ToString();
            txtIrregularDays.Text = employee.IrregularVacationDays.ToString();
            txtExperienceDays.Text = employee.ExperienceVacationDays.ToString();
            txtBonusDays.Text = employee.BonusVacationDays.ToString();

            RefreshVacationsGrid();
            UpdateVacationTotals();
        }

        public List<VacationEntry> GetVacations() => _vacations;
        public List<VacationEntry> GetArchive() => _vacationsArchive;

        public void SaveData()
        {
            if (_employee == null) return;

            _employee.ContractVacationDays = int.TryParse(txtContractDays.Text, out int c) ? c : 0;
            _employee.HarmfulVacationDays = int.TryParse(txtHarmfulDays.Text, out int h) ? h : 0;
            _employee.IrregularVacationDays = int.TryParse(txtIrregularDays.Text, out int i) ? i : 0;
            _employee.ExperienceVacationDays = int.TryParse(txtExperienceDays.Text, out int e) ? e : 0;
            _employee.BonusVacationDays = int.TryParse(txtBonusDays.Text, out int b) ? b : 0;
        }

        private void RefreshVacationsGrid()
        {
            dgVacations.ItemsSource = null;
            dgVacations.ItemsSource = _vacations;
            UpdateTotalRemaining();
        }

        private void UpdateTotalRemaining()
        {
            int totalRemaining = _vacations.Sum(v => v.RemainingDays);
            txtTotalRemaining.Text = totalRemaining.ToString();
            txtTotalRemaining.Foreground = totalRemaining < 0
                ? System.Windows.Media.Brushes.Red
                : System.Windows.Media.Brushes.Green;
        }

        private void UpdateVacationTotals()
        {
            try
            {
                if (txtContractDays == null) return;

                int contract = ParseInt(txtContractDays.Text);
                int harmful = ParseInt(txtHarmfulDays.Text);
                int irregular = ParseInt(txtIrregularDays.Text);
                int experience = ParseInt(txtExperienceDays.Text);
                int bonus = ParseInt(txtBonusDays.Text);

                int totalAdditional = contract + harmful + irregular + experience + bonus;
                txtTotalAdditional.Text = totalAdditional.ToString();
                int totalYear = 24 + totalAdditional;
                txtTotalYear.Text = totalYear.ToString();

                foreach (var vacation in _vacations)
                {
                    vacation.AdditionalDays = totalAdditional;
                    vacation.BasicDays = 24;
                }
                RefreshVacationsGrid();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private int ParseInt(string text) => string.IsNullOrEmpty(text) ? 0 : int.Parse(text);

        private void AdditionalVacation_TextChanged(object sender, TextChangedEventArgs e)
            => UpdateVacationTotals();

        /// <summary>
        /// Добавление нового периода отпуска
        /// </summary>
        private void AddVacationRow_Click(object sender, RoutedEventArgs e)
        {
            if (_employee == null) return;

            DateTime? periodStart = _employee.HireDate;
            if (!periodStart.HasValue)
            {
                periodStart = new DateTime(DateTime.Today.Year, 1, 1);
            }

            int additionalDays = ParseInt(txtTotalAdditional.Text);

            var dialog = new VacationEditWindow(null, periodStart, additionalDays);
            dialog.VacationSaved += (vacation) =>
            {
                _vacations.Add(vacation);
                RefreshVacationsGrid();
                UpdateVacationTotals();
            };
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        /// <summary>
        /// Добавить отпуск в выбранный период
        /// </summary>
        private void AddVacationToPeriod_Click(object sender, RoutedEventArgs e)
        {
            if (dgVacations.SelectedItem is not VacationEntry selectedPeriod)
            {
                MessageBox.Show("Сначала выберите период отпуска в таблице!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверяем остаток дней
            if (selectedPeriod.RemainingDays <= 0)
            {
                MessageBox.Show($"В периоде {selectedPeriod.PeriodName} остаток дней = {selectedPeriod.RemainingDays}.\n" +
                    "Нельзя добавить отпуск, так как все дни уже использованы.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Открываем окно для ввода нового отпуска в этом периоде
            var dialog = new VacationEntryWindow(selectedPeriod);
            dialog.VacationEntrySaved += (days, startDate, basis) =>
            {
                // Добавляем использованные дни к существующим
                selectedPeriod.UsedDays += days;
                selectedPeriod.StartDate = startDate;
                selectedPeriod.EndDate = startDate.AddDays(days - 1);
                selectedPeriod.Basis = basis;

                RefreshVacationsGrid();
                UpdateVacationTotals();

                MessageBox.Show($"Отпуск на {days} дней добавлен в период {selectedPeriod.PeriodName}.\n" +
                    $"Остаток дней: {selectedPeriod.RemainingDays}",
                    "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            };
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        /// <summary>
        /// При изменении даты начала отпуска автоматически рассчитываем дату окончания
        /// </summary>
        private void VacationStartDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var datePicker = sender as DatePicker;
            if (datePicker?.DataContext is VacationEntry vacation)
            {
                if (vacation.StartDate.HasValue && vacation.UsedDays > 0)
                {
                    vacation.EndDate = vacation.StartDate.Value.AddDays(vacation.UsedDays - 1);
                    RefreshVacationsGrid();

                    // Проверка правила 14 дней
                    if (vacation.UsedDays < 14 && vacation.UsedDays > 0)
                    {
                        MessageBox.Show(
                            "Внимание! Согласно законодательству, одна часть отпуска должна быть не менее 14 календарных дней.\n\n" +
                            $"Текущая часть отпуска составляет {vacation.UsedDays} дней.\n\n" +
                            "Рекомендуем скорректировать количество дней.",
                            "Проверка законодательства",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                }
                else if (vacation.StartDate.HasValue)
                {
                    vacation.EndDate = null;
                    RefreshVacationsGrid();
                }
            }
        }

        /// <summary>
        /// Редактирование по двойному клику
        /// </summary>
        private void DgVacations_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgVacations.SelectedItem is VacationEntry selected)
            {
                EditVacation(selected);
            }
        }

        /// <summary>
        /// Кнопка редактирования
        /// </summary>
        private void EditVacationRow_Click(object sender, RoutedEventArgs e)
        {
            if (dgVacations.SelectedItem is VacationEntry selected)
            {
                EditVacation(selected);
            }
            else
            {
                MessageBox.Show("Выберите отпуск для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Редактирование отпуска
        /// </summary>
        private void EditVacation(VacationEntry selected)
        {
            int additionalDays = ParseInt(txtTotalAdditional.Text);

            var dialog = new VacationEditWindow(selected, null, additionalDays);
            dialog.VacationSaved += (vacation) =>
            {
                var index = _vacations.FindIndex(v => v.Id == vacation.Id);
                if (index >= 0)
                {
                    _vacations[index] = vacation;
                    RefreshVacationsGrid();
                    UpdateVacationTotals();
                }
            };
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        /// <summary>
        /// Кнопка удаления
        /// </summary>
        private void DeleteVacationRow_Click(object sender, RoutedEventArgs e)
        {
            if (dgVacations.SelectedItem is VacationEntry selected)
            {
                if (MessageBox.Show($"Удалить период отпуска {selected.PeriodName}?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _vacations.Remove(selected);
                    RefreshVacationsGrid();
                    UpdateVacationTotals();
                }
            }
            else
            {
                MessageBox.Show("Выберите отпуск для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void dgVacations_SelectionChanged()
        {

        }
    }
}