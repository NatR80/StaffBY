using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StaffBY.App.ViewModels;

namespace StaffBY.App.Views.UserControls
{
    public partial class PaymentControl : UserControl
    {
        private List<PayrollPaymentViewModel> _payments = new List<PayrollPaymentViewModel>();
        public event Action<string>? StatusMessageChanged;
        public List<PayrollAccrualViewModel> Accruals { get; set; } = new List<PayrollAccrualViewModel>();

        public PaymentControl()
        {
            InitializeComponent();
            SetDefaultDates();
        }

        public void SetAccruals(List<PayrollAccrualViewModel> accruals) => Accruals = accruals;

        private void SetDefaultDates()
        {
            var today = DateTime.Now;
            var firstDayPrev = new DateTime(today.Year, today.Month - 1, 1);
            dpStart.SelectedDate = firstDayPrev;
            dpEnd.SelectedDate = firstDayPrev.AddMonths(1).AddDays(-1);
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Accruals.Any())
            {
                StatusMessageChanged?.Invoke("Сначала рассчитайте начисления!");
                return;
            }

            _payments = Accruals.Select(a => new PayrollPaymentViewModel
            {
                EmployeeId = a.EmployeeId,
                PersonalNumber = a.PersonalNumber,
                FullName = a.FullName,
                AccruedAmount = a.AccruedAmount,
                IncomeTax = Math.Round(a.AccruedAmount * 0.13m, 2),
                Contributions = Math.Round(a.AccruedAmount * 0.01m, 2),
                NetAmount = Math.Round(a.AccruedAmount * 0.86m, 2),
                Deductions = new List<DeductionDetail>
                {
                    new DeductionDetail { Name = "Подоходный налог", Amount = Math.Round(a.AccruedAmount * 0.13m, 2), Description = "13%" },
                    new DeductionDetail { Name = "Взносы в ФСЗН", Amount = Math.Round(a.AccruedAmount * 0.01m, 2), Description = "1%" }
                }
            }).ToList();

            PaymentDataGrid.ItemsSource = _payments;
            decimal total = _payments.Sum(p => p.NetAmount);
            StatusMessageChanged?.Invoke($"К выплате: {_payments.Count} сотрудников, всего: {total:N2} руб.");
        }

        private void PaymentDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PaymentDataGrid.SelectedItem is PayrollPaymentViewModel selected)
            {
                txtDetails.Text = $"Сотрудник: {selected.FullName}\n" +
                                  $"Начислено: {selected.AccruedAmount:N2} руб.\n" +
                                  $"Подоходный налог: {selected.IncomeTax:N2} руб.\n" +
                                  $"Взносы: {selected.Contributions:N2} руб.\n" +
                                  $"К выплате: {selected.NetAmount:N2} руб.";
            }
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            if (PaymentDataGrid.SelectedItem is PayrollPaymentViewModel selected)
                MessageBox.Show($"Печать расчетного листка\n{selected.FullName}\nК выплате: {selected.NetAmount:N2} руб.", "Печать");
            else
                MessageBox.Show("Выберите сотрудника", "Внимание");
        }
    }
}