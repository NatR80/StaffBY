using StaffBY.App.Models;
using StaffBY.App.ViewModels;
using System.Windows.Controls;
using System.Windows;

namespace StaffBY.App.Views.UserControls.EmployeeCard
{
    public partial class EmployeeCardMain : UserControl
    {
        private EmployeeViewModel? _employee;

        public event EventHandler<EmployeeViewModel>? EmployeeSaved;

        public EmployeeCardMain()
        {
            InitializeComponent();
        }

        public void LoadData(EmployeeViewModel employee)
        {
            _employee = employee ?? throw new ArgumentNullException(nameof(employee));
            txtTitle.Text = $"ЛИЧНАЯ КАРТОЧКА РАБОТНИКА (форма Т-2) - {employee.FullName}";

            CommonInfo.LoadData(employee);
            Employment.LoadData(employee);
            Military.LoadData(employee);
            Vacations.LoadData(employee);
            Family.LoadData(employee?.FamilyMembers ?? new System.Collections.Generic.List<FamilyMemberEntry>());
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_employee == null) return;

            CommonInfo.SaveData();
            Employment.SaveData();
            Military.SaveData();
            Vacations.SaveData();
            _employee.FamilyMembers = Family.GetFamilyMembers();

            EmployeeSaved?.Invoke(this, _employee);

            var window = Window.GetWindow(this);
            window?.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window?.Close();
        }
    }
}