using System;
using System.Windows;
using System.Windows.Controls;
using StaffBY.App.ViewModels;
using System.Collections.Generic;
using StaffBY.App.Models;

namespace StaffBY.App.Views.UserControls.EmployeeCard
{
    public partial class CommonInfoControl : UserControl
    {
        public EmployeeViewModel Employee { get; set; }

        public CommonInfoControl()
        {
            InitializeComponent();
        }

        public void LoadData(EmployeeViewModel employee)
        {
            Employee = employee;

            txtPersonalNumber.Text = employee.PersonalNumber;
            cmbGender.SelectedIndex = employee.Gender == "М" ? 0 : employee.Gender == "Ж" ? 1 : -1;
            txtLastName.Text = employee.LastName;
            txtFirstName.Text = employee.FirstName;
            txtPatronymic.Text = employee.Patronymic;
            dpBirthDate.SelectedDate = employee.BirthDate;
            txtBirthPlace.Text = employee.BirthPlace;
            cmbCitizenship.Text = employee.Citizenship;
            cmbEducation.Text = employee.Education;
            txtEducationInstitution.Text = employee.EducationInstitution;
            dpEducationEndDate.SelectedDate = employee.EducationEndDate;
            txtQualification.Text = employee.Qualification;

            txtPassportSeries.Text = employee.PassportSeries;
            txtPassportNumber.Text = employee.PassportNumber;
            dpPassportIssueDate.SelectedDate = employee.PassportIssueDate;
            txtPassportIssuedBy.Text = employee.PassportIssuedBy;
            dpPassportExpiryDate.SelectedDate = employee.PassportExpiryDate;
            txtIdentificationNumber.Text = employee.IdentificationNumber;
            txtInsuranceNumber.Text = employee.InsuranceNumber;
            cmbMaritalStatus.Text = employee.MaritalStatus;
            txtHomeAddress.Text = employee.HomeAddress;
            txtRegistrationAddress.Text = employee.RegistrationAddress;
            txtPhone.Text = employee.Phone;
            txtEmail.Text = employee.Email;
            txtAdditionalInfo.Text = employee.AdditionalInfo;

            UpdateShortName();
        }

        public void SaveData()
        {
            if (Employee == null) return;

            Employee.PersonalNumber = txtPersonalNumber.Text;
            Employee.Gender = (cmbGender.SelectedItem as ComboBoxItem)?.Content.ToString();
            Employee.LastName = txtLastName.Text;
            Employee.FirstName = txtFirstName.Text;
            Employee.Patronymic = txtPatronymic.Text;
            Employee.BirthDate = dpBirthDate.SelectedDate ?? DateTime.Now;
            Employee.BirthPlace = txtBirthPlace.Text;
            Employee.Citizenship = (cmbCitizenship.SelectedItem as ComboBoxItem)?.Content.ToString();
            Employee.Education = (cmbEducation.SelectedItem as ComboBoxItem)?.Content.ToString();
            Employee.EducationInstitution = txtEducationInstitution.Text;
            Employee.EducationEndDate = dpEducationEndDate.SelectedDate;
            Employee.Qualification = txtQualification.Text;
            Employee.PassportSeries = txtPassportSeries.Text;
            Employee.PassportNumber = txtPassportNumber.Text;
            Employee.PassportIssueDate = dpPassportIssueDate.SelectedDate;
            Employee.PassportIssuedBy = txtPassportIssuedBy.Text;
            Employee.PassportExpiryDate = dpPassportExpiryDate.SelectedDate;
            Employee.IdentificationNumber = txtIdentificationNumber.Text;
            Employee.InsuranceNumber = txtInsuranceNumber.Text;
            Employee.MaritalStatus = (cmbMaritalStatus.SelectedItem as ComboBoxItem)?.Content.ToString();
            Employee.HomeAddress = txtHomeAddress.Text;
            Employee.RegistrationAddress = txtRegistrationAddress.Text;
            Employee.Phone = txtPhone.Text;
            Employee.Email = txtEmail.Text;
            Employee.AdditionalInfo = txtAdditionalInfo.Text;
        }

        private void UpdateShortName()
        {
            string lastName = txtLastName.Text;
            string firstNameInitial = string.IsNullOrEmpty(txtFirstName.Text) ? "" : txtFirstName.Text[0] + ".";
            string patronymicInitial = string.IsNullOrEmpty(txtPatronymic.Text) ? "" : txtPatronymic.Text[0] + ".";
            txtShortName.Text = $"{lastName} {firstNameInitial}{patronymicInitial}".Trim();
        }

        private void TxtLastName_TextChanged(object sender, TextChangedEventArgs e) => UpdateShortName();
        private void TxtFirstName_TextChanged(object sender, TextChangedEventArgs e) => UpdateShortName();
        private void TxtPatronymic_TextChanged(object sender, TextChangedEventArgs e) => UpdateShortName();
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
    }
}