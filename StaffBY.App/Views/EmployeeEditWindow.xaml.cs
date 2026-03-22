using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StaffBY.App.ViewModels;
using StaffBY.App.Views;

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
            UpdateVacationBalance();
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
            // TODO: Загрузить из базы данных
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

        private void LoadVacations()
        {
            // TODO: Загрузить отпуска сотрудника из базы данных
            dgVacations.ItemsSource = _vacations;
        }

        private void LoadFamilyMembers()
        {
            // TODO: Загрузить членов семьи из базы данных
            dgFamilyMembers.ItemsSource = _familyMembers;
        }

        private void UpdateVacationBalance()
        {
            // Расчет остатка дней отпуска (28 дней в год минус использованные)
            int usedDays = _vacations.Sum(v => v.DaysCount);
            int balance = 28 - usedDays;
            txtVacationBalance.Text = balance.ToString();

            if (balance < 0)
                txtVacationBalance.Foreground = System.Windows.Media.Brushes.Red;
            else
                txtVacationBalance.Foreground = System.Windows.Media.Brushes.Green;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка обязательных полей
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

            // Сохраняем все данные в _employee
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
            _employee.Phone = txtPhone.Text;
            _employee.Email = txtEmail.Text;

            // Трудоустройство
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

            // Воинский учет
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

            // Вызываем событие сохранения
            EmployeeSaved?.Invoke(this, _employee);

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void AddVacation_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Открыть окно добавления отпуска
            var vacationWindow = new VacationEditWindow();
            vacationWindow.VacationSaved += (s, vacation) =>
            {
                _vacations.Add(vacation);
                LoadVacations();
                UpdateVacationBalance();
            };
            vacationWindow.Owner = this;
            vacationWindow.ShowDialog();
        }

        private void EditVacation_Click(object sender, RoutedEventArgs e)
        {
            if (dgVacations.SelectedItem is VacationEntry selectedVacation)
            {
                var vacationWindow = new VacationEditWindow(selectedVacation);
                vacationWindow.VacationSaved += (s, vacation) =>
                {
                    var index = _vacations.FindIndex(v => v.Id == vacation.Id);
                    if (index >= 0)
                    {
                        _vacations[index] = vacation;
                        LoadVacations();
                        UpdateVacationBalance();
                    }
                };
                vacationWindow.Owner = this;
                vacationWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите отпуск для редактирования", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteVacation_Click(object sender, RoutedEventArgs e)
        {
            if (dgVacations.SelectedItem is VacationEntry selectedVacation)
            {
                var result = MessageBox.Show($"Удалить запись об отпуске?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _vacations.Remove(selectedVacation);
                    LoadVacations();
                    UpdateVacationBalance();
                }
            }
            else
            {
                MessageBox.Show("Выберите отпуск для удаления", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddFamilyMember_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Открыть окно добавления члена семьи
            var familyWindow = new FamilyMemberEditWindow();
            familyWindow.MemberSaved += (s, member) =>
            {
                _familyMembers.Add(member);
                LoadFamilyMembers();
            };
            familyWindow.Owner = this;
            familyWindow.ShowDialog();
        }

        private void RemoveFamilyMember_Click(object sender, RoutedEventArgs e)
        {
            if (dgFamilyMembers.SelectedItem is FamilyMemberEntry selectedMember)
            {
                var result = MessageBox.Show($"Удалить члена семьи '{selectedMember.FullName}'?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _familyMembers.Remove(selectedMember);
                    LoadFamilyMembers();
                }
            }
            else
            {
                MessageBox.Show("Выберите члена семьи для удаления", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    // Вспомогательные классы
    public class VacationEntry
    {
        public int Id { get; set; }
        public string VacationType { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DaysCount { get; set; }
        public string Basis { get; set; } = string.Empty;
    }

    public class FamilyMemberEntry
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public int BirthYear { get; set; }
        public string WorkPlace { get; set; } = string.Empty;
    }
}