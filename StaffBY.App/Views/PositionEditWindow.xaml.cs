using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StaffBY.App.ViewModels;


namespace StaffBY.App.Views
{
    public partial class PositionEditWindow : Window
    {
        private PositionViewModel _position;
        private bool _isEdit;
        private List<string> _departments;

        public PositionViewModel? Result { get; private set; }

        public PositionEditWindow(PositionViewModel? position = null, List<string>? departments = null)
        {
            InitializeComponent();

            _departments = departments ?? new List<string>();

            // Заполняем список подразделений
            cmbDepartment.Items.Clear();
            foreach (var dept in _departments)
            {
                cmbDepartment.Items.Add(dept);
            }
            cmbDepartment.IsEditable = true;

            if (position != null)
            {
                _isEdit = true;
                _position = position;
                LoadData();
                Title = "Редактирование должности";
            }
            else
            {
                _isEdit = false;
                _position = new PositionViewModel();
                Title = "Новая должность";
            }
        }

        private void LoadData()
        {
            cmbDepartment.Text = _position.DepartmentName;
            txtName.Text = _position.Name;
            txtStaffUnits.Text = _position.NumberOfPositions.ToString();

            // Выбор ставки
            decimal rateValue = _position.Rate;
            foreach (ComboBoxItem item in cmbRate.Items)
            {
                if (decimal.TryParse(item.Content.ToString(), out decimal itemValue) && itemValue == rateValue)
                {
                    cmbRate.SelectedItem = item;
                    break;
                }
            }

            cmbWorkWeek.Text = _position.WorkWeek;
            cmbCategory.Text = _position.Category;
            txtSalary.Text = _position.Salary.ToString("N2");
            txtAllowance.Text = _position.Allowance.ToString("N2");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(cmbDepartment.Text))
            {
                MessageBox.Show("Введите подразделение", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите должность", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Сохраняем данные
            _position.DepartmentName = cmbDepartment.Text;
            _position.Name = txtName.Text;

            if (int.TryParse(txtStaffUnits.Text, out int units))
                _position.NumberOfPositions = units;

            // Безопасное преобразование ставки
            if (cmbRate.SelectedItem is ComboBoxItem rateItem)
            {
                string rateString = rateItem.Content.ToString();
                if (decimal.TryParse(rateString, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal rate))
                {
                    _position.Rate = rate;
                }
                else
                {
                    _position.Rate = 1.0m;
                }
            }

            _position.WorkWeek = cmbWorkWeek.Text;
            _position.Category = cmbCategory.Text;

            if (decimal.TryParse(txtSalary.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal salary))
                _position.Salary = salary;

            if (decimal.TryParse(txtAllowance.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal allowance))
                _position.Allowance = allowance;

            // Для новой должности устанавливаем Id позже
            if (!_isEdit)
                _position.Id = 0;

            Result = _position;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}