using System;
using System.Windows;

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
            txtBirthYear.Text = _member.BirthYear.ToString();
            txtWorkPlace.Text = _member.WorkPlace;
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

            if (cmbRelationship.SelectedItem == null)
            {
                MessageBox.Show("Выберите степень родства", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка года рождения
            int birthYear = 0;
            if (!string.IsNullOrWhiteSpace(txtBirthYear.Text))
            {
                if (!int.TryParse(txtBirthYear.Text, out birthYear) || birthYear < 1900 || birthYear > DateTime.Now.Year)
                {
                    MessageBox.Show("Введите корректный год рождения (1900-2026)", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtBirthYear.Focus();
                    return;
                }
            }

            // Сохраняем данные
            _member.FullName = txtFullName.Text;
            _member.Relationship = (cmbRelationship.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();
            _member.BirthYear = birthYear;
            _member.WorkPlace = txtWorkPlace.Text;

            // Генерируем ID для нового члена семьи
            if (!_isEditMode)
            {
                _member.Id = DateTime.Now.Millisecond;
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
