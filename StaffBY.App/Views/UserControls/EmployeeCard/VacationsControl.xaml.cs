using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StaffBY.App.ViewModels;
using StaffBY.App.Models;


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

        private void AddVacationRow_Click(object sender, RoutedEventArgs e)
        {
            var newVacation = new VacationEntry
            {
                Id = DateTime.Now.GetHashCode(),
                PeriodStart = DateTime.Now,
                PeriodEnd = DateTime.Now.AddYears(1).AddDays(-1),
                BasicDays = 24,
                UsedDays = 0
            };
            _vacations.Add(newVacation);
            RefreshVacationsGrid();
        }

        private void EditVacationRow_Click(object sender, RoutedEventArgs e)
        {
            if (dgVacations.SelectedItem is VacationEntry selected)
            {
                var dialog = new VacationEditWindow(selected);
                dialog.VacationSaved += (vacation) =>
                {
                    var index = _vacations.FindIndex(v => v.Id == vacation.Id);
                    if (index >= 0) _vacations[index] = vacation;
                    
                    RefreshVacationsGrid();
                    UpdateVacationTotals();
                };
                dialog.Owner = Window.GetWindow(this);
                dialog.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите отпуск для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteVacationRow_Click(object sender, RoutedEventArgs e)
        {
            if (dgVacations.SelectedItem is VacationEntry selected)
            {
                if (MessageBox.Show("Удалить запись об отпуске?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _vacations.Remove(selected);
                    RefreshVacationsGrid();
                    UpdateVacationTotals();
                }
            }
        }
    }
}