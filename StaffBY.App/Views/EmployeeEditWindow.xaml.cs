using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StaffBY.App.ViewModels;

namespace StaffBY.App.Views
{
    public partial class EmployeeEditWindow : Window
    {
        private EmployeeViewModel _employee;
        private bool _isEditMode;

        // Коллекция для отпусков
        private List<VacationEntry> _vacations = new List<VacationEntry>();

        // Коллекция для членов семьи
        private List<FamilyMemberEntry> _familyMembers = new List<FamilyMemberEntry>();

        public event EventHandler<EmployeeViewModel> EmployeeSaved;

        public EmployeeEditWindow(EmployeeViewModel employee = null)
        {
            InitializeComponent();

            if (employee == null)
            {
                _isEditMode = false;
                _employee = new EmployeeViewModel
                {
                    PersonalNumber = GeneratePersonalNumber()
                };
                Title = "Добавление нового сотрудника";
            }
            else
            {
                _isEditMode = true;
                _employee = employee;
                Title = $"Редактирование сотрудника: {employee.LastName} {employee.FirstName}";
            }

            LoadData();
            LoadPositionsAndDepartments();
            LoadVacations();
            LoadFamilyMembers();
            UpdateVacationTotals();

            // Подписываемся на события изменения дополнительных отпусков
            txtContractVacation.TextChanged += AdditionalVacation_TextChanged;
            txtHarmfulVacation.TextChanged += AdditionalVacation_TextChanged;
            txtIrregularVacation.TextChanged += AdditionalVacation_TextChanged;
            txtExperienceVacation.TextChanged += AdditionalVacation_TextChanged;
            txtBonusVacation.TextChanged += AdditionalVacation_TextChanged;
        }

        private string GeneratePersonalNumber()
        {
            return DateTime.Now.ToString("yyMMddHHmmss");
        }

        private void LoadData()
        {
            // Общие сведения
            txtPersonalNumber.Text = _employee.PersonalNumber;

            if (_employee.Gender == "М")
                cmbGender.SelectedIndex = 0;
            else if (_employee.Gender == "Ж")
                cmbGender.SelectedIndex = 1;

            txtLastName.Text = _employee.LastName;
            txtFirstName.Text = _employee.FirstName;
            txtPatronymic.Text = _employee.Patronymic;
            dpBirthDate.SelectedDate = _employee.BirthDate;
            txtBirthPlace.Text = _employee.BirthPlace;

            if (!string.IsNullOrEmpty(_employee.Citizenship))
                cmbCitizenship.Text = _employee.Citizenship;

            if (!string.IsNullOrEmpty(_employee.Education))
                cmbEducation.Text = _employee.Education;

            txtEducationInstitution.Text = _employee.EducationInstitution;
            dpEducationEndDate.SelectedDate = _employee.EducationEndDate;
            txtQualification.Text = _employee.Qualification;

            // Паспортные данные
            txtPassportSeries.Text = _employee.PassportSeries;
            txtPassportNumber.Text = _employee.PassportNumber;
            dpPassportIssueDate.SelectedDate = _employee.PassportIssueDate;
            txtPassportIssuedBy.Text = _employee.PassportIssuedBy;
            dpPassportExpiryDate.SelectedDate = _employee.PassportExpiryDate;
            txtIdentificationNumber.Text = _employee.IdentificationNumber;
            txtInsuranceNumber.Text = _employee.InsuranceNumber;

            if (!string.IsNullOrEmpty(_employee.MaritalStatus))
                cmbMaritalStatus.Text = _employee.MaritalStatus;

            txtHomeAddress.Text = _employee.HomeAddress;
            txtRegistrationAddress.Text = _employee.RegistrationAddress;
            txtPhone.Text = _employee.Phone;
            txtEmail.Text = _employee.Email;

            // Трудоустройство
            if (_employee.PositionId > 0)
                cmbPosition.SelectedValue = _employee.PositionId;

            if (_employee.DepartmentId > 0)
                cmbDepartment.SelectedValue = _employee.DepartmentId;

            dpHireDate.SelectedDate = _employee.HireDate;
            txtSalary.Text = _employee.Salary?.ToString("N2");

            if (!string.IsNullOrEmpty(_employee.EmploymentType))
                cmbEmploymentType.Text = _employee.EmploymentType;

            if (!string.IsNullOrEmpty(_employee.ContractType))
                cmbContractType.Text = _employee.ContractType;

            dpContractEndDate.SelectedDate = _employee.ContractEndDate;

            // Стаж
            if (_employee.HireDate.HasValue)
            {
                var experience = CalculateWorkExperience(_employee.HireDate.Value);
                txtWorkExperience.Text = experience;
                txtExperienceHere.Text = experience;
            }

            // Воинский учет
            if (!string.IsNullOrEmpty(_employee.MilitaryCategory))
                cmbMilitaryCategory.Text = _employee.MilitaryCategory;

            if (!string.IsNullOrEmpty(_employee.MilitaryComposition))
                cmbMilitaryComposition.Text = _employee.MilitaryComposition;

            txtMilitaryRank.Text = _employee.MilitaryRank;
            txtMilitaryVUS.Text = _employee.MilitaryVUS;

            if (!string.IsNullOrEmpty(_employee.MilitaryFitness))
                cmbFitness.Text = _employee.MilitaryFitness;

            txtMilitaryCommissariat.Text = _employee.MilitaryCommissariat;
            chkSpecialRegistration.IsChecked = _employee.IsSpecialRegistration;
            txtMobilizationNumber.Text = _employee.MobilizationNumber;
            dpRegistrationDate.SelectedDate = _employee.RegistrationDate;
            chkRemovedFromRegistry.IsChecked = _employee.IsRemovedFromRegistry;
            dpRemovalDate.SelectedDate = _employee.RemovalDate;

            // Дополнительная информация
            txtAdditionalInfo.Text = _employee.AdditionalInfo;
        }

        private string CalculateWorkExperience(DateTime hireDate)
        {
            var today = DateTime.Now;
            var years = today.Year - hireDate.Year;
            var months = today.Month - hireDate.Month;

            if (months < 0)
            {
                years--;
                months += 12;
            }

            return $"{years} лет {months} месяцев";
        }

        private void LoadPositionsAndDepartments()
        {
            cmbPosition.ItemsSource = new List<dynamic>
            {
                new { Id = 1, Name = "Директор" },
                new { Id = 2, Name = "Главный бухгалтер" },
                new { Id = 3, Name = "Инженер-программист" },
                new { Id = 4, Name = "Менеджер по персоналу" }
            };

            cmbDepartment.ItemsSource = new List<dynamic>
            {
                new { Id = 1, Name = "Руководство" },
                new { Id = 2, Name = "Бухгалтерия" },
                new { Id = 3, Name = "IT отдел" },
                new { Id = 4, Name = "Отдел кадров" }
            };
        }

        // ==================== ОТПУСКА ====================

        /// <summary>
        /// Класс для хранения записи об отпуске
        /// </summary>
        public class VacationEntry
        {
            public int Id { get; set; }
            public DateTime PeriodStart { get; set; }
            public DateTime PeriodEnd { get; set; }
            public int BasicDays { get; set; } = 24;
            public int AdditionalDays { get; set; }
            public int TotalDays => BasicDays + AdditionalDays;
            public int UsedDays { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int RemainingDays => TotalDays - UsedDays;
            public string Basis { get; set; } = string.Empty;
            public string RemainingColor => RemainingDays <= 0 ? "Red" : "Green";
        }

        private void LoadVacations()
        {
            dgVacations.ItemsSource = _vacations;
            UpdateVacationTotals();
        }

        private void UpdateVacationTotals()
        {
            try
            {
                int contract = string.IsNullOrEmpty(txtContractVacation.Text) ? 0 : int.Parse(txtContractVacation.Text);
                int harmful = string.IsNullOrEmpty(txtHarmfulVacation.Text) ? 0 : int.Parse(txtHarmfulVacation.Text);
                int irregular = string.IsNullOrEmpty(txtIrregularVacation.Text) ? 0 : int.Parse(txtIrregularVacation.Text);
                int experience = string.IsNullOrEmpty(txtExperienceVacation.Text) ? 0 : int.Parse(txtExperienceVacation.Text);
                int bonus = string.IsNullOrEmpty(txtBonusVacation.Text) ? 0 : int.Parse(txtBonusVacation.Text);

                int totalAdditional = contract + harmful + irregular + experience + bonus;
                txtTotalAdditionalVacation.Text = totalAdditional.ToString();

                int totalYear = 24 + totalAdditional;
                txtTotalYearVacation.Text = totalYear.ToString();

                int totalUsed = _vacations.Sum(v => v.UsedDays);
                int totalRemaining = totalYear - totalUsed;
                txtCurrentVacationBalance.Text = totalRemaining.ToString();

                if (totalRemaining < 0)
                    txtCurrentVacationBalance.Foreground = System.Windows.Media.Brushes.Red;
                else
                    txtCurrentVacationBalance.Foreground = System.Windows.Media.Brushes.Green;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private void AddVacation_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция добавления отпуска в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditVacation_Click(object sender, RoutedEventArgs e)
        {
            if (dgVacations.SelectedItem is VacationEntry selected)
            {
                MessageBox.Show($"Редактирование отпуска за период {selected.PeriodStart:dd.MM.yyyy} - {selected.PeriodEnd:dd.MM.yyyy}",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Выберите отпуск для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteVacation_Click(object sender, RoutedEventArgs e)
        {
            if (dgVacations.SelectedItem is VacationEntry selected)
            {
                var result = MessageBox.Show($"Удалить запись об отпуске за период {selected.PeriodStart:dd.MM.yyyy} - {selected.PeriodEnd:dd.MM.yyyy}?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _vacations.Remove(selected);
                    LoadVacations();
                    UpdateVacationTotals();
                }
            }
            else
            {
                MessageBox.Show("Выберите отпуск для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RecalculateVacationBalance_Click(object sender, RoutedEventArgs e)
        {
            UpdateVacationTotals();
            MessageBox.Show("Остатки отпуска пересчитаны", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AdditionalVacation_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateVacationTotals();
        }

        // ==================== СОСТАВ СЕМЬИ ====================

        private void LoadFamilyMembers()
        {
            dgFamilyMembers.ItemsSource = _familyMembers;
        }

        private void AddFamilyMember_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция добавления члена семьи в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RemoveFamilyMember_Click(object sender, RoutedEventArgs e)
        {
            if (dgFamilyMembers.SelectedItem is FamilyMemberEntry selected)
            {
                var result = MessageBox.Show($"Удалить члена семьи '{selected.FullName}'?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _familyMembers.Remove(selected);
                    LoadFamilyMembers();
                }
            }
            else
            {
                MessageBox.Show("Выберите члена семьи для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // ==================== СОХРАНЕНИЕ И ОТМЕНА ====================

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Введите фамилию сотрудника", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtLastName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Введите имя сотрудника", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFirstName.Focus();
                return;
            }

            if (dpBirthDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату рождения", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                dpBirthDate.Focus();
                return;
            }

            _employee.PersonalNumber = txtPersonalNumber.Text;
            _employee.Gender = (cmbGender.SelectedItem as ComboBoxItem)?.Content.ToString();
            _employee.LastName = txtLastName.Text;
            _employee.FirstName = txtFirstName.Text;
            _employee.Patronymic = txtPatronymic.Text;
            _employee.BirthDate = dpBirthDate.SelectedDate.Value;
            _employee.BirthPlace = txtBirthPlace.Text;
            _employee.Citizenship = (cmbCitizenship.SelectedItem as ComboBoxItem)?.Content.ToString();
            _employee.Education = (cmbEducation.SelectedItem as ComboBoxItem)?.Content.ToString();
            _employee.EducationInstitution = txtEducationInstitution.Text;
            _employee.EducationEndDate = dpEducationEndDate.SelectedDate;
            _employee.Qualification = txtQualification.Text;
            _employee.PassportSeries = txtPassportSeries.Text;
            _employee.PassportNumber = txtPassportNumber.Text;
            _employee.PassportIssueDate = dpPassportIssueDate.SelectedDate;
            _employee.PassportIssuedBy = txtPassportIssuedBy.Text;
            _employee.PassportExpiryDate = dpPassportExpiryDate.SelectedDate;
            _employee.IdentificationNumber = txtIdentificationNumber.Text;
            _employee.InsuranceNumber = txtInsuranceNumber.Text;
            _employee.MaritalStatus = (cmbMaritalStatus.SelectedItem as ComboBoxItem)?.Content.ToString();
            _employee.HomeAddress = txtHomeAddress.Text;
            _employee.RegistrationAddress = txtRegistrationAddress.Text;
            _employee.Phone = txtPhone.Text;
            _employee.Email = txtEmail.Text;

            if (cmbPosition.SelectedItem != null)
            {
                dynamic pos = cmbPosition.SelectedItem;
                _employee.PositionId = pos.Id;
                _employee.PositionName = pos.Name;
            }

            if (cmbDepartment.SelectedItem != null)
            {
                dynamic dept = cmbDepartment.SelectedItem;
                _employee.DepartmentId = dept.Id;
                _employee.DepartmentName = dept.Name;
            }

            _employee.HireDate = dpHireDate.SelectedDate;

            if (decimal.TryParse(txtSalary.Text, out decimal salary))
                _employee.Salary = salary;

            _employee.EmploymentType = (cmbEmploymentType.SelectedItem as ComboBoxItem)?.Content.ToString();
            _employee.ContractType = (cmbContractType.SelectedItem as ComboBoxItem)?.Content.ToString();
            _employee.ContractEndDate = dpContractEndDate.SelectedDate;

            _employee.MilitaryCategory = (cmbMilitaryCategory.SelectedItem as ComboBoxItem)?.Content.ToString();
            _employee.MilitaryComposition = (cmbMilitaryComposition.SelectedItem as ComboBoxItem)?.Content.ToString();
            _employee.MilitaryRank = txtMilitaryRank.Text;
            _employee.MilitaryVUS = txtMilitaryVUS.Text;
            _employee.MilitaryFitness = (cmbFitness.SelectedItem as ComboBoxItem)?.Content.ToString();
            _employee.MilitaryCommissariat = txtMilitaryCommissariat.Text;
            _employee.IsSpecialRegistration = chkSpecialRegistration.IsChecked ?? false;
            _employee.MobilizationNumber = txtMobilizationNumber.Text;
            _employee.RegistrationDate = dpRegistrationDate.SelectedDate;
            _employee.IsRemovedFromRegistry = chkRemovedFromRegistry.IsChecked ?? false;
            _employee.RemovalDate = dpRemovalDate.SelectedDate;

            _employee.AdditionalInfo = txtAdditionalInfo.Text;

            EmployeeSaved?.Invoke(this, _employee);
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // ==================== ФОТО И ФАМИЛИЯ И.О. ====================

        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Выберите фотографию сотрудника",
                Filter = "Изображения (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (dialog.ShowDialog() == true)
            {
                MessageBox.Show($"Фото добавлено: {System.IO.Path.GetFileName(dialog.FileName)}",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void UpdateShortName()
        {
            string lastName = txtLastName.Text;
            string firstNameInitial = string.IsNullOrEmpty(txtFirstName.Text) ? "" : txtFirstName.Text[0] + ".";
            string patronymicInitial = string.IsNullOrEmpty(txtPatronymic.Text) ? "" : txtPatronymic.Text[0] + ".";

            txtShortName.Text = $"{lastName} {firstNameInitial}{patronymicInitial}".Trim();
        }

        private void TxtLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateShortName();
        }

        private void TxtFirstName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateShortName();
        }

        private void TxtPatronymic_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateShortName();
        }
    }

    /// <summary>
    /// Класс для хранения члена семьи
    /// </summary>
    public class FamilyMemberEntry
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public int BirthYear { get; set; }
        public string WorkPlace { get; set; } = string.Empty;
    }
}