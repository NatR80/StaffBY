using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StaffBY.App.ViewModels;
using StaffBY.Domain.Entities;
using StaffBY.Business.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace StaffBY.App.Views
{
    public partial class MainWindow : Window
    {
        private User _currentUser;
        private IEmployeeService? _employeeService;

        private List<EmployeeViewModel> _employees = new List<EmployeeViewModel>();
        private List<PositionViewModel> _positions = new List<PositionViewModel>();
        private List<VacationViewModel> _vacations = new List<VacationViewModel>();

        // Новые коллекции для новых модулей
        private OrganizationViewModel _organization = new OrganizationViewModel();
        private List<TimesheetViewModel> _timesheets = new List<TimesheetViewModel>();
        private List<PayrollAccrualViewModel> _accruals = new List<PayrollAccrualViewModel>();
        private List<PayrollPaymentViewModel> _payments = new List<PayrollPaymentViewModel>();

        public MainWindow(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;

            try
            {
                var app = Application.Current as App;
                if (app != null)
                {
                    _employeeService = app.Services.GetService(typeof(IEmployeeService)) as IEmployeeService;
                }
            }
            catch
            {
                _employeeService = null;
            }

            UserInfoText.Text = $"Пользователь: {currentUser.Username}";

            LoadDemoData();
            LoadOrganizationData();
            RefreshAllData();

            // Устанавливаем даты по умолчанию для периодов
            SetDefaultDates();

            StatusText.Text = $"Добро пожаловать, {currentUser.Username}!";
        }

        private void SetDefaultDates()
        {
            // Табель - текущий месяц
            var today = DateTime.Now;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            dpTimesheetStart.SelectedDate = firstDayOfMonth;
            dpTimesheetEnd.SelectedDate = lastDayOfMonth;

            // Начисление ЗП - предыдущий месяц
            var firstDayOfPrevMonth = firstDayOfMonth.AddMonths(-1);
            var lastDayOfPrevMonth = firstDayOfPrevMonth.AddMonths(1).AddDays(-1);

            dpPayrollStart.SelectedDate = firstDayOfPrevMonth;
            dpPayrollEnd.SelectedDate = lastDayOfPrevMonth;

            dpPaymentStart.SelectedDate = firstDayOfPrevMonth;
            dpPaymentEnd.SelectedDate = lastDayOfPrevMonth;
        }

        private void LoadDemoData()
        {
            _employees = new List<EmployeeViewModel>
            {
                new EmployeeViewModel
                {
                    Id = 1,
                    PersonalNumber = "001",
                    LastName = "Иванов",
                    FirstName = "Иван",
                    Patronymic = "Иванович",
                    PositionName = "Директор",
                    DepartmentName = "Руководство",
                    BirthDate = new DateTime(1980, 5, 15),
                    Phone = "+375291234567",
                    Salary = 5000
                },
                new EmployeeViewModel
                {
                    Id = 2,
                    PersonalNumber = "002",
                    LastName = "Петрова",
                    FirstName = "Анна",
                    Patronymic = "Сергеевна",
                    PositionName = "Главный бухгалтер",
                    DepartmentName = "Бухгалтерия",
                    BirthDate = new DateTime(1985, 8, 22),
                    Phone = "+375292345678",
                    Salary = 3500
                },
                new EmployeeViewModel
                {
                    Id = 3,
                    PersonalNumber = "003",
                    LastName = "Сидоров",
                    FirstName = "Петр",
                    Patronymic = "Алексеевич",
                    PositionName = "Инженер-программист",
                    DepartmentName = "IT отдел",
                    BirthDate = new DateTime(1990, 3, 10),
                    Phone = "+375293456789",
                    Salary = 2500
                },
                new EmployeeViewModel
                {
                    Id = 4,
                    PersonalNumber = "004",
                    LastName = "Козлова",
                    FirstName = "Елена",
                    Patronymic = "Владимировна",
                    PositionName = "Менеджер по персоналу",
                    DepartmentName = "Отдел кадров",
                    BirthDate = new DateTime(1988, 7, 18),
                    Phone = "+375294567890",
                    Salary = 2000
                }
            };

            _positions = new List<PositionViewModel>
            {
                new PositionViewModel { Id = 1, Name = "Директор", DepartmentName = "Руководство", Salary = 5000, NumberOfPositions = 1, Allowance = 1000 },
                new PositionViewModel { Id = 2, Name = "Главный бухгалтер", DepartmentName = "Бухгалтерия", Salary = 3500, NumberOfPositions = 1, Allowance = 500 },
                new PositionViewModel { Id = 3, Name = "Инженер-программист", DepartmentName = "IT отдел", Salary = 2500, NumberOfPositions = 3, Allowance = 300 },
                new PositionViewModel { Id = 4, Name = "Менеджер по персоналу", DepartmentName = "Отдел кадров", Salary = 2000, NumberOfPositions = 1, Allowance = 200 }
            };

            _vacations = new List<VacationViewModel>
            {
                new VacationViewModel
                {
                    Id = 1,
                    EmployeeFullName = "Иванов Иван Иванович",
                    VacationType = "Основной",
                    StartDate = new DateTime(2026, 6, 1),
                    EndDate = new DateTime(2026, 6, 28),
                    DaysCount = 28,
                    Basis = "Приказ №123 от 01.05.2026"
                }
            };
        }

        // ==================== ОРГАНИЗАЦИЯ ====================

        private void LoadOrganizationData()
        {
            try
            {
                _organization = new OrganizationViewModel
                {
                    Id = 1,
                    FullName = "Общество с ограниченной ответственностью \"Пример\"",
                    ShortName = "ООО \"Пример\"",
                    UNP = "123456789",
                    LegalAddress = "г. Минск, ул. Примерная, д. 1",
                    DirectorId = null
                };

                var directors = _employees.Where(e => !e.IsArchived).ToList();
                cmbDirector.ItemsSource = directors;

                txtFullName.Text = _organization.FullName;
                txtShortName.Text = _organization.ShortName;
                txtUNP.Text = _organization.UNP;
                txtAddress.Text = _organization.LegalAddress;
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Ошибка загрузки организации: {ex.Message}";
            }
        }

        private void SaveOrganizationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _organization.FullName = txtFullName.Text;
                _organization.ShortName = txtShortName.Text;
                _organization.UNP = txtUNP.Text;
                _organization.LegalAddress = txtAddress.Text;

                if (cmbDirector.SelectedItem is EmployeeViewModel director)
                {
                    _organization.DirectorId = director.Id;
                    _organization.DirectorName = director.FullName;
                }

                _organization.UpdatedAt = DateTime.Now;

                StatusText.Text = "Данные организации сохранены";
                MessageBox.Show("Данные организации сохранены!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Ошибка сохранения: {ex.Message}";
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshOrganizationButton_Click(object sender, RoutedEventArgs e)
        {
            LoadOrganizationData();
            StatusText.Text = "Данные организации обновлены";
        }

        // ==================== ТАБЕЛЬ ====================

        private void GenerateTimesheetButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StatusText.Text = "Формирование табеля...";

                var startDate = dpTimesheetStart.SelectedDate ?? DateTime.Now.AddMonths(-1);
                var endDate = dpTimesheetEnd.SelectedDate ?? DateTime.Now;

                var activeEmployees = _employees.Where(emp => !emp.IsArchived).ToList();

                _timesheets = new List<TimesheetViewModel>();

                var random = new Random();

                foreach (var emp in activeEmployees)
                {
                    int totalDays = (endDate - startDate).Days + 1;
                    int workedDays = random.Next(15, totalDays);
                    int sickDays = random.Next(0, 5);
                    int vacationDays = random.Next(0, 10);
                    int absentDays = totalDays - workedDays - sickDays - vacationDays;
                    if (absentDays < 0) absentDays = 0;

                    _timesheets.Add(new TimesheetViewModel
                    {
                        EmployeeId = emp.Id,
                        PersonalNumber = emp.PersonalNumber,
                        FullName = emp.FullName,
                        WorkedDays = workedDays,
                        SickDays = sickDays,
                        VacationDays = vacationDays,
                        AbsentDays = absentDays
                    });
                }

                TimesheetDataGrid.ItemsSource = _timesheets;
                StatusText.Text = $"Табель сформирован для {_timesheets.Count} сотрудников за {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Ошибка формирования табеля: {ex.Message}";
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImportTimesheetButton_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Импорт табеля из Excel...";
            MessageBox.Show("Функция импорта табеля в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ==================== НАЧИСЛЕНИЕ ЗАРПЛАТЫ ====================

        private void CalculateAccrualsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StatusText.Text = "Расчет начислений...";

                var startDate = dpPayrollStart.SelectedDate ?? DateTime.Now.AddMonths(-1);
                var endDate = dpPayrollEnd.SelectedDate ?? DateTime.Now;

                var activeEmployees = _employees.Where(emp => !emp.IsArchived).ToList();

                _accruals = new List<PayrollAccrualViewModel>();

                foreach (var emp in activeEmployees)
                {
                    int workedDays = 20;

                    var timesheet = _timesheets.FirstOrDefault(t => t.EmployeeId == emp.Id);
                    if (timesheet != null)
                    {
                        workedDays = timesheet.WorkedDays;
                    }

                    decimal salary = emp.Salary.HasValue && emp.Salary.Value > 0 ? emp.Salary.Value : 2000m;
                    decimal dailyRate = salary / 20;
                    decimal accruedAmount = Math.Round(dailyRate * workedDays, 2);

                    _accruals.Add(new PayrollAccrualViewModel
                    {
                        EmployeeId = emp.Id,
                        PersonalNumber = emp.PersonalNumber,
                        FullName = emp.FullName,
                        PositionName = emp.PositionName,
                        Salary = salary,
                        WorkedDays = workedDays,
                        AccruedAmount = accruedAmount
                    });
                }

                AccrualsDataGrid.ItemsSource = _accruals;
                StatusText.Text = $"Рассчитано начислений для {_accruals.Count} сотрудников";

                decimal totalAccrued = _accruals.Sum(a => a.AccruedAmount);
                StatusText.Text = $"Рассчитано начислений: {_accruals.Count} сотрудников, всего: {totalAccrued:N2} руб.";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Ошибка расчета: {ex.Message}";
                MessageBox.Show($"Ошибка расчета: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportAccrualsButton_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Экспорт начислений в Excel...";
            MessageBox.Show("Функция экспорта в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ==================== ВЫПЛАТА ЗАРПЛАТЫ ====================

        private void CalculatePaymentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StatusText.Text = "Расчет к выплате...";

                var startDate = dpPaymentStart.SelectedDate ?? DateTime.Now.AddMonths(-1);
                var endDate = dpPaymentEnd.SelectedDate ?? DateTime.Now;

                if (_accruals == null || _accruals.Count == 0)
                {
                    CalculateAccrualsButton_Click(sender, e);
                }

                _payments = new List<PayrollPaymentViewModel>();

                foreach (var accrual in _accruals)
                {
                    decimal incomeTax = Math.Round(accrual.AccruedAmount * 0.13m, 2);
                    decimal contributions = Math.Round(accrual.AccruedAmount * 0.01m, 2);
                    decimal netAmount = accrual.AccruedAmount - incomeTax - contributions;

                    _payments.Add(new PayrollPaymentViewModel
                    {
                        EmployeeId = accrual.EmployeeId,
                        PersonalNumber = accrual.PersonalNumber,
                        FullName = accrual.FullName,
                        AccruedAmount = accrual.AccruedAmount,
                        IncomeTax = incomeTax,
                        Contributions = contributions,
                        NetAmount = netAmount,
                        Deductions = new List<DeductionDetail>
                        {
                            new DeductionDetail { Name = "Подоходный налог", Amount = incomeTax, Description = "13%" },
                            new DeductionDetail { Name = "Взносы в ФСЗН", Amount = contributions, Description = "1%" }
                        }
                    });
                }

                PaymentDataGrid.ItemsSource = _payments;

                decimal totalNet = _payments.Sum(p => p.NetAmount);
                StatusText.Text = $"Рассчитано к выплате: {_payments.Count} сотрудников, всего: {totalNet:N2} руб.";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Ошибка расчета: {ex.Message}";
                MessageBox.Show($"Ошибка расчета: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PaymentDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PaymentDataGrid.SelectedItem is PayrollPaymentViewModel selected)
            {
                var details = $"Сотрудник: {selected.FullName}\n" +
                              $"Табельный номер: {selected.PersonalNumber}\n" +
                              $"\n--- Начислено ---\n" +
                              $"{selected.AccruedAmount:N2} руб.\n" +
                              $"\n--- Удержания ---\n";

                if (selected.Deductions != null)
                {
                    foreach (var deduction in selected.Deductions)
                    {
                        details += $"{deduction.Name}: {deduction.Amount:N2} руб. ({deduction.Description})\n";
                    }
                }

                details += $"\n--- Итого к выплате ---\n" +
                           $"{selected.NetAmount:N2} руб.\n" +
                           $"\n--- Детализация ---\n" +
                           $"Прописью: {NumberToWords(selected.NetAmount)}";

                txtPaymentDetails.Text = details;
            }
            else
            {
                txtPaymentDetails.Text = "Выберите сотрудника для просмотра деталей расчета";
            }
        }

        private void PrintPayslipButton_Click(object sender, RoutedEventArgs e)
        {
            if (PaymentDataGrid.SelectedItem is PayrollPaymentViewModel selected)
            {
                StatusText.Text = $"Печать расчетного листка для {selected.FullName}";
                MessageBox.Show($"Печать расчетного листка для {selected.FullName}\n\n" +
                                $"Начислено: {selected.AccruedAmount:N2} руб.\n" +
                                $"Подоходный налог: {selected.IncomeTax:N2} руб.\n" +
                                $"Взносы: {selected.Contributions:N2} руб.\n" +
                                $"К выплате: {selected.NetAmount:N2} руб.",
                                "Расчетный листок",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Выберите сотрудника для печати расчетного листка",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private string NumberToWords(decimal number)
        {
            if (number == 0) return "ноль рублей";

            int rubles = (int)number;
            int kopecks = (int)((number - rubles) * 100);

            string rublesStr = RublesToWords(rubles);
            string kopecksStr = $"{kopecks:D2} коп.";

            return $"{rublesStr} {kopecksStr}";
        }

        private string RublesToWords(int number)
        {
            if (number == 0) return "ноль рублей";

            string[] units = { "", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять" };
            string[] teens = { "десять", "одиннадцать", "двенадцать", "тринадцать", "четырнадцать", "пятнадцать", "шестнадцать", "семнадцать", "восемнадцать", "девятнадцать" };
            string[] tens = { "", "", "двадцать", "тридцать", "сорок", "пятьдесят", "шестьдесят", "семьдесят", "восемьдесят", "девяносто" };
            string[] hundreds = { "", "сто", "двести", "триста", "четыреста", "пятьсот", "шестьсот", "семьсот", "восемьсот", "девятьсот" };

            string result = "";

            int thousands = number / 1000;
            int remainder = number % 1000;

            if (thousands > 0)
            {
                if (thousands == 1)
                    result += "одна тысяча ";
                else if (thousands == 2)
                    result += "две тысячи ";
                else
                    result += ConvertHundreds(thousands, units, teens, tens, hundreds) + " тысяч ";
            }

            result += ConvertHundreds(remainder, units, teens, tens, hundreds);

            int lastDigit = number % 10;
            int lastTwoDigits = number % 100;

            if (lastTwoDigits >= 11 && lastTwoDigits <= 14)
                result += "рублей";
            else if (lastDigit == 1)
                result += "рубль";
            else if (lastDigit >= 2 && lastDigit <= 4)
                result += "рубля";
            else
                result += "рублей";

            return result.Trim();
        }

        private string ConvertHundreds(int number, string[] units, string[] teens, string[] tens, string[] hundreds)
        {
            if (number == 0) return "";

            string result = "";

            int hundred = number / 100;
            int remainder = number % 100;

            if (hundred > 0)
                result += hundreds[hundred] + " ";

            if (remainder >= 10 && remainder <= 19)
            {
                result += teens[remainder - 10] + " ";
            }
            else
            {
                int ten = remainder / 10;
                int unit = remainder % 10;

                if (ten > 0)
                    result += tens[ten] + " ";
                if (unit > 0)
                    result += units[unit] + " ";
            }

            return result;
        }

        // ==================== СУЩЕСТВУЮЩИЕ МЕТОДЫ (без изменений) ====================

        private void RefreshAllData()
        {
            RefreshEmployeesGrid();
            RefreshPositionsGrid();
            RefreshVacationsGrid();
            RefreshArchivedEmployeesGrid();
            RefreshEmployeeComboBox();
        }

        private void RefreshEmployeesGrid()
        {
            EmployeesDataGrid.ItemsSource = _employees.Where(e => !e.IsArchived).ToList();
            RecordCountText.Text = $"Записей: {EmployeesDataGrid.ItemsSource.Cast<object>().Count()}";
        }

        private void RefreshPositionsGrid()
        {
            PositionsDataGrid.ItemsSource = _positions.ToList();
        }

        private void RefreshVacationsGrid()
        {
            VacationsDataGrid.ItemsSource = _vacations.ToList();
        }

        private void RefreshArchivedEmployeesGrid()
        {
            ArchivedEmployeesDataGrid.ItemsSource = _employees.Where(e => e.IsArchived).ToList();
        }

        private void RefreshEmployeeComboBox()
        {
            var activeEmployees = _employees.Where(e => !e.IsArchived).ToList();
            EmployeeComboBox.ItemsSource = activeEmployees;

            if (activeEmployees.Count > 0)
                EmployeeComboBox.SelectedIndex = 0;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
                var authWindow = new AuthWindow();
                authWindow.Show();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                RefreshEmployeesGrid();
            }
            else
            {
                var filtered = _employees
                    .Where(e => !e.IsArchived &&
                        (e.LastName.ToLower().Contains(searchText) ||
                         e.FirstName.ToLower().Contains(searchText) ||
                         e.Patronymic.ToLower().Contains(searchText) ||
                         e.PersonalNumber.Contains(searchText)))
                    .ToList();

                EmployeesDataGrid.ItemsSource = filtered;
                RecordCountText.Text = $"Записей: {filtered.Count} (найдено)";
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox_TextChanged(sender, null);
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EmployeeEditWindow();
            editWindow.EmployeeSaved += (s, employee) =>
            {
                employee.Id = _employees.Count > 0 ? _employees.Max(x => x.Id) + 1 : 1;
                _employees.Add(employee);
                RefreshEmployeesGrid();
                StatusText.Text = $"Сотрудник {employee.LastName} {employee.FirstName} добавлен";
            };
            editWindow.Owner = this;
            editWindow.ShowDialog();
        }

        private void EmployeesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (EmployeesDataGrid.SelectedItem is EmployeeViewModel selectedEmployee)
            {
                var editWindow = new EmployeeEditWindow(selectedEmployee);
                editWindow.EmployeeSaved += (s, employee) =>
                {
                    var index = _employees.FindIndex(x => x.Id == employee.Id);
                    if (index >= 0)
                    {
                        _employees[index] = employee;
                        RefreshEmployeesGrid();
                        StatusText.Text = $"Сотрудник {employee.LastName} {employee.FirstName} обновлен";
                    }
                };
                editWindow.Owner = this;
                editWindow.ShowDialog();
            }
        }

        // Заглушки для остальных методов
        private void ExportToExcelButton_Click(object sender, RoutedEventArgs e) { }
        private void AddPositionButton_Click(object sender, RoutedEventArgs e) { }
        private void EditPositionButton_Click(object sender, RoutedEventArgs e) { }
        private void DeletePositionButton_Click(object sender, RoutedEventArgs e) { }
        private void AddVacationButton_Click(object sender, RoutedEventArgs e) { }
        private void VacationScheduleButton_Click(object sender, RoutedEventArgs e) { }
        private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
        private void GenerateDocumentButton_Click(object sender, RoutedEventArgs e) { }
        private void EmployeeListReportButton_Click(object sender, RoutedEventArgs e) { }
        private void PersonnelMovementReportButton_Click(object sender, RoutedEventArgs e) { }
        private void BirthdaysReportButton_Click(object sender, RoutedEventArgs e) { }
        private void VacationsReportButton_Click(object sender, RoutedEventArgs e) { }
        private void MilitaryReportButton_Click(object sender, RoutedEventArgs e) { }
        private string GenerateDocument(string? docType, EmployeeViewModel? employee) => "";
    }
}