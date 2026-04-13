using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StaffBY.App.Models;
using StaffBY.App.ViewModels;
using StaffBY.App.ViewModels.StaffBY.App.ViewModels;

namespace StaffBY.App.Views.UserControls.EmployeeCard
{
    public partial class EmploymentControl : UserControl
    {
        private EmployeeViewModel? _employee;
        private bool _isLoading = false;
        // Добавить в начало класса:
        private List<PositionViewModel> _positions = new List<PositionViewModel>();
        
        public EmploymentControl()
        {
            InitializeComponent();
        }

        // Добавить НОВЫЙ метод для загрузки списка должностей
        public void LoadPositions(List<PositionViewModel> positions)
        {
            _positions = positions;
            cmbPosition.Items.Clear();
            foreach (var pos in positions)
            {
                cmbPosition.Items.Add(pos);
            }
        }
        // Загрузка данных сотрудника
        public void LoadData(EmployeeViewModel employee)
        {
            _isLoading = true;
            _employee = employee;

            if (employee == null) return;

            // Основные поля
            cmbPosition.Text = employee.PositionName;
            cmbDepartment.Text = employee.DepartmentName;
            dpHireDate.SelectedDate = employee.HireDate;
            txtSalary.Text = employee.Salary?.ToString("N2");
            txtAllowance.Text = employee.Allowance?.ToString("N2") ?? "0.00";
            UpdateTotalSalary();

            // Ставка и часы
            if (employee.Rate.HasValue)
            {
                string rateText = employee.Rate.Value.ToString("0.00");
                foreach (ComboBoxItem item in cmbRate.Items)
                {
                    if (item.Content.ToString().Contains(rateText))
                    {
                        cmbRate.SelectedItem = item;
                        break;
                    }
                }
            }

            if (employee.HoursPerDay > 0)
            {
                foreach (ComboBoxItem item in cmbHours.Items)
                {
                    if (item.Content.ToString() == employee.HoursPerDay.ToString())
                    {
                        cmbHours.SelectedItem = item;
                        break;
                    }
                }
            }

            txtGrade.Text = employee.Grade?.ToString();
            txtCoefficient.Text = employee.Coefficient?.ToString("N2") ?? "1.0";
            txtIntensity.Text = employee.IntensityPercent?.ToString() ?? "0";
            txtContractPercent.Text = employee.ContractPercent?.ToString() ?? "0";

            // Форма занятости
            if (!string.IsNullOrEmpty(employee.EmploymentType))
                cmbEmploymentType.Text = employee.EmploymentType;

            // Тип договора
            if (!string.IsNullOrEmpty(employee.ContractType))
                cmbContractType.Text = employee.ContractType;

            dpContractEnd.SelectedDate = employee.ContractEndDate;
            UpdateExperience();

            // Рабочая неделя
            if (!string.IsNullOrEmpty(employee.WorkWeek))
                cmbWorkWeek.Text = employee.WorkWeek;

            // Категория
            if (!string.IsNullOrEmpty(employee.Category))
                cmbCategory.Text = employee.Category;

            // Условия труда
            if (!string.IsNullOrEmpty(employee.WorkConditions))
                cmbWorkConditions.Text = employee.WorkConditions;

            chkIsPensioner.IsChecked = employee.IsPensioner;
            chkIsDisabled.IsChecked = employee.IsDisabled;
            dpDismissalDate.SelectedDate = employee.DismissalDate;

            // Дополнительные сведения (используем поле из EmployeeViewModel)
            txtEmploymentAdditionalInfo.Text = employee.EmploymentAdditionalInfo;

            // Загрузка таблиц
            LoadEmploymentHistory();
            LoadAdvancedTraining();
            LoadCertification();

            _isLoading = false;
        }

        // Сохранение данных
        public void SaveData()
        {
            if (_employee == null) return;

            _employee.PositionName = cmbPosition.Text;
            _employee.DepartmentName = cmbDepartment.Text;
            _employee.HireDate = dpHireDate.SelectedDate;
            _employee.Salary = ParseDecimal(txtSalary.Text);
            _employee.Allowance = ParseDecimal(txtAllowance.Text);

            if (cmbRate.SelectedItem is ComboBoxItem rateItem)
                _employee.Rate = ParseRateFromString(rateItem.Content.ToString());

            if (cmbHours.SelectedItem is ComboBoxItem hoursItem)
                _employee.HoursPerDay = int.Parse(hoursItem.Content.ToString());

            _employee.Grade = ParseInt(txtGrade.Text);
            _employee.Coefficient = ParseDecimal(txtCoefficient.Text);
            _employee.IntensityPercent = ParseInt(txtIntensity.Text);
            _employee.ContractPercent = ParseInt(txtContractPercent.Text);
            _employee.EmploymentType = cmbEmploymentType.Text;
            _employee.ContractType = cmbContractType.Text;
            _employee.ContractEndDate = dpContractEnd.SelectedDate;
            _employee.WorkWeek = cmbWorkWeek.Text;
            _employee.Category = cmbCategory.Text;
            _employee.WorkConditions = cmbWorkConditions.Text;
            _employee.IsPensioner = chkIsPensioner.IsChecked ?? false;
            _employee.IsDisabled = chkIsDisabled.IsChecked ?? false;
            _employee.DismissalDate = dpDismissalDate.SelectedDate;
            _employee.EmploymentAdditionalInfo = txtEmploymentAdditionalInfo.Text;
        }

        // ========== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ==========

        private void UpdateTotalSalary()
        {
            decimal salary = ParseDecimal(txtSalary.Text);
            decimal allowance = ParseDecimal(txtAllowance.Text);
            decimal total = salary + allowance;
            tbTotalSalary.Text = total.ToString("N2");
        }

        private void UpdateExperience()
        {
            if (dpHireDate.SelectedDate == null)
            {
                tbExperience.Text = "0 лет 0 мес 0 дн";
                return;
            }

            DateTime startDate = dpHireDate.SelectedDate.Value;
            DateTime endDate = dpDismissalDate.SelectedDate ?? DateTime.Today;

            if (endDate < startDate) endDate = DateTime.Today;

            int years = endDate.Year - startDate.Year;
            int months = endDate.Month - startDate.Month;
            int days = endDate.Day - startDate.Day;

            if (days < 0)
            {
                months--;
                days += DateTime.DaysInMonth(endDate.Year, endDate.Month);
            }
            if (months < 0)
            {
                years--;
                months += 12;
            }

            tbExperience.Text = $"{years} лет {months} мес {days} дн";
        }

        private decimal ParseDecimal(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return 0;
            if (decimal.TryParse(input, out decimal result)) return result;
            return 0;
        }

        private int ParseInt(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return 0;
            if (int.TryParse(input, out int result)) return result;
            return 0;
        }

        private decimal ParseRateFromString(string? rateText)
        {
            if (string.IsNullOrWhiteSpace(rateText)) return 1;
            var parts = rateText.Split(' ');
            if (parts.Length > 0 && decimal.TryParse(parts[0], out decimal result))
                return result;
            return 1;
        }

        // ========== ЗАГРУЗКА ТАБЛИЦ ==========

        private void LoadEmploymentHistory()
        {
            dgEmploymentHistory.ItemsSource = _employee?.EmploymentHistory ?? new List<EmploymentHistoryDto>();
        }

        private void LoadAdvancedTraining()
        {
            dgAdvancedTraining.ItemsSource = _employee?.AdvancedTraining ?? new List<AdvancedTrainingDto>();
        }

        private void LoadCertification()
        {
            dgCertification.ItemsSource = _employee?.Certifications ?? new List<CertificationDto>();
        }

        // ========== ОБРАБОТЧИКИ СОБЫТИЙ ==========

        private void CmbPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoading || _employee == null) return;

            if (cmbPosition.SelectedItem is PositionViewModel selected)
            {
                _employee.PositionId = selected.Id;
                _employee.PositionName = selected.Name;
                _employee.DepartmentName = selected.DepartmentName;

                // Подтягиваем данные из штатного расписания
                _employee.Salary = selected.Salary;
                _employee.Allowance = selected.Allowance;
                _employee.Rate = selected.Rate;
                _employee.WorkWeek = selected.WorkWeek;
                _employee.Category = selected.Category;

                // Обновляем UI
                cmbDepartment.Text = selected.DepartmentName;
                txtSalary.Text = selected.Salary.ToString("N2");
                txtAllowance.Text = selected.Allowance.ToString("N2");

                // Обновляем ставку
                string rateText = selected.Rate.ToString("0.00");
                foreach (ComboBoxItem item in cmbRate.Items)
                {
                    if (item.Content.ToString().Contains(rateText))
                    {
                        cmbRate.SelectedItem = item;
                        break;
                    }
                }

                cmbWorkWeek.Text = selected.WorkWeek;
                cmbCategory.Text = selected.Category;
                UpdateTotalSalary();
            }
        }

        private void CmbDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoading && _employee != null)
            {
                // Обновить выбранный отдел
            }
        }

        private void DpHireDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoading)
                UpdateExperience();
        }

        private void TxtSalary_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoading)
                UpdateTotalSalary();
        }

        private void TxtAllowance_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoading)
                UpdateTotalSalary();
        }

        private void CmbContractType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoading && cmbContractType.SelectedItem is ComboBoxItem item)
            {
                string tag = item.Tag?.ToString() ?? "";
                bool isContract = tag == "02";

                // Для контракта поле "Срок действия" становится обязательным
                dpContractEnd.IsEnabled = isContract;
                if (!isContract)
                    dpContractEnd.SelectedDate = null;
            }
        }

        // ========== ОБРАБОТЧИКИ ТАБЛИЦ ==========

        private void DgEmploymentHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool hasSelection = dgEmploymentHistory.SelectedItem != null;
            btnEditHistory.IsEnabled = hasSelection;
            btnDeleteHistory.IsEnabled = hasSelection;
        }

        private void BtnAddHistory_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Добавление записи о назначении/перемещении");
        }

        private void BtnEditHistory_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Редактирование записи");
        }

        private void BtnDeleteHistory_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить выбранную запись?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // TODO: Удалить запись
            }
        }

        private void DgAdvancedTraining_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool hasSelection = dgAdvancedTraining.SelectedItem != null;
            btnEditTraining.IsEnabled = hasSelection;
            btnDeleteTraining.IsEnabled = hasSelection;
        }

        private void BtnAddTraining_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Добавление записи о повышении квалификации/переподготовке");
        }

        private void BtnEditTraining_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Редактирование записи");
        }

        private void BtnDeleteTraining_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить выбранную запись?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // TODO: Удалить запись
            }
        }

        private void DgCertification_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool hasSelection = dgCertification.SelectedItem != null;
            btnEditCertification.IsEnabled = hasSelection;
            btnDeleteCertification.IsEnabled = hasSelection;
        }

        private void BtnAddCertification_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Добавление записи об аттестации");
        }

        private void BtnEditCertification_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Редактирование записи");
        }

        private void BtnDeleteCertification_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить выбранную запись?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // TODO: Удалить запись
            }
        }

        private void TxtEmploymentAdditionalInfo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoading && _employee != null)
                _employee.EmploymentAdditionalInfo = txtEmploymentAdditionalInfo.Text;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
            MessageBox.Show("Данные сохранены!", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_employee != null)
                LoadData(_employee);
        }
    }
}