using System.Windows.Controls;
using StaffBY.App.ViewModels;
using System.Collections.Generic;
using StaffBY.App.Models;

namespace StaffBY.App.Views.UserControls.EmployeeCard
{
    public partial class MilitaryControl : UserControl
    {
        public EmployeeViewModel Employee { get; set; }

        public MilitaryControl()
        {
            InitializeComponent();
        }

        public void LoadData(EmployeeViewModel employee)
        {
            Employee = employee;

            cmbMilitaryCategory.Text = employee.MilitaryCategory;
            cmbMilitaryComposition.Text = employee.MilitaryComposition;
            txtMilitaryRank.Text = employee.MilitaryRank;
            txtMilitaryVUS.Text = employee.MilitaryVUS;
            cmbFitness.Text = employee.MilitaryFitness;
            txtMilitaryCommissariat.Text = employee.MilitaryCommissariat;
            chkSpecialRegistration.IsChecked = employee.IsSpecialRegistration;
            txtMobilizationNumber.Text = employee.MobilizationNumber;
            dpRegistrationDate.SelectedDate = employee.RegistrationDate;
            chkRemovedFromRegistry.IsChecked = employee.IsRemovedFromRegistry;
            dpRemovalDate.SelectedDate = employee.RemovalDate;
        }

        public void SaveData()
        {
            if (Employee == null) return;

            Employee.MilitaryCategory = (cmbMilitaryCategory.SelectedItem as ComboBoxItem)?.Content.ToString();
            Employee.MilitaryComposition = (cmbMilitaryComposition.SelectedItem as ComboBoxItem)?.Content.ToString();
            Employee.MilitaryRank = txtMilitaryRank.Text;
            Employee.MilitaryVUS = txtMilitaryVUS.Text;
            Employee.MilitaryFitness = (cmbFitness.SelectedItem as ComboBoxItem)?.Content.ToString();
            Employee.MilitaryCommissariat = txtMilitaryCommissariat.Text;
            Employee.IsSpecialRegistration = chkSpecialRegistration.IsChecked ?? false;
            Employee.MobilizationNumber = txtMobilizationNumber.Text;
            Employee.RegistrationDate = dpRegistrationDate.SelectedDate;
            Employee.IsRemovedFromRegistry = chkRemovedFromRegistry.IsChecked ?? false;
            Employee.RemovalDate = dpRemovalDate.SelectedDate;
        }
    }
}