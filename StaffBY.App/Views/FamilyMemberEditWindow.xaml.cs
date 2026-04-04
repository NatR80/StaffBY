using System;
using System.Windows;
using System.Windows.Controls;
using StaffBY.App.Models;

namespace StaffBY.App.Views
{
    public partial class FamilyMemberEditWindow : Window
    {
        private FamilyMemberEntry _member;
        private bool _isEditMode;

        public event EventHandler<FamilyMemberEntry> MemberSaved;

        public FamilyMemberEditWindow(FamilyMemberEntry member = null)
        {
            InitializeComponent();

            if (member == null)
            {
                _isEditMode = false;
                _member = new FamilyMemberEntry();
                Title = "Добавление члена семьи";
            }
            else
            {
                _isEditMode = true;
                _member = member;
                Title = "Редактирование члена семьи";
                LoadData();
            }
        }

        private void LoadData()
        {
            txtFullName.Text = _member.FullName;
            cmbRelationship.Text = _member.Relationship;
            dpBirthDate.SelectedDate = _member.BirthDate != DateTime.MinValue ? _member.BirthDate : (DateTime?)null;
            txtWorkPlace.Text = _member.WorkPlace;

            // Установка документа в комбобокс
            if (!string.IsNullOrEmpty(_member.Document))
            {
                cmbDocument.Text = _member.Document;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка обязательных полей
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Введите ФИО члена семьи", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFullName.Focus();
                return;
            }

            if (cmbRelationship.SelectedItem == null && string.IsNullOrWhiteSpace(cmbRelationship.Text))
            {
                MessageBox.Show("Выберите или введите степень родства", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка даты рождения
            DateTime birthDate = DateTime.MinValue;
            if (dpBirthDate.SelectedDate.HasValue)
            {
                birthDate = dpBirthDate.SelectedDate.Value;
                if (birthDate > DateTime.Now)
                {
                    MessageBox.Show("Дата рождения не может быть в будущем", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    dpBirthDate.Focus();
                    return;
                }
            }

            // Сохраняем данные
            _member.FullName = txtFullName.Text.Trim();
            _member.Relationship = cmbRelationship.Text;
            _member.BirthDate = birthDate;
            _member.WorkPlace = txtWorkPlace.Text.Trim();
            _member.Document = cmbDocument.Text.Trim();

            // Генерируем ID для нового члена семьи
            if (!_isEditMode)
            {
                _member.Id = Math.Abs(Guid.NewGuid().GetHashCode());
            }

            MemberSaved?.Invoke(this, _member);
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}