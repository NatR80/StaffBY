using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StaffBY.App.ViewModels;

namespace StaffBY.App.Views.UserControls
{
    public partial class OrganizationControl : UserControl
    {
        private OrganizationViewModel _organization = new OrganizationViewModel();

        public event Action<string>? StatusMessageChanged;

        public OrganizationControl()
        {
            InitializeComponent();
            LoadData();
        }

        public void LoadData()
        {
            try
            {
                _organization = new OrganizationViewModel
                {
                    Id = 1,
                    FullName = "Общество с ограниченной ответственностью \"Пример\"",
                    ShortName = "ООО \"Пример\"",
                    UNP = "123456789",
                    LegalAddress = "г. Минск, ул. Примерная, д. 1"
                };

                txtFullName.Text = _organization.FullName;
                txtShortName.Text = _organization.ShortName;
                txtUNP.Text = _organization.UNP;
                txtAddress.Text = _organization.LegalAddress;

                StatusMessageChanged?.Invoke("Данные организации загружены");
            }
            catch (Exception ex)
            {
                StatusMessageChanged?.Invoke($"Ошибка: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _organization.FullName = txtFullName.Text;
                _organization.ShortName = txtShortName.Text;
                _organization.UNP = txtUNP.Text;
                _organization.LegalAddress = txtAddress.Text;

                StatusMessageChanged?.Invoke("Данные организации сохранены");
                MessageBox.Show("Данные организации сохранены!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessageChanged?.Invoke($"Ошибка: {ex.Message}");
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            StatusMessageChanged?.Invoke("Данные организации обновлены");
        }
    }
}