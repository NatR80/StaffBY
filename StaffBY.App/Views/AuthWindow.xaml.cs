using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using StaffBY.App.ViewModels;
using StaffBY.Domain.Entities;

namespace StaffBY.App.Views
{
    public partial class AuthWindow : Window
    {
        private AuthViewModel _viewModel;

        public AuthWindow()
        {
            InitializeComponent();
        }

        public AuthWindow(AuthViewModel viewModel) : this()
        {
            _viewModel = viewModel;
            DataContext = viewModel;
        }

        // Обработчик нажатия кнопки "Войти"
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AuthViewModel viewModel)
            {
                // Обновляем логин в ViewModel
                viewModel.Username = txtLogin.Text;
                // Вызываем метод входа, передавая пароль
                // Вся логика аутентификации внутри ExecuteLogin
                viewModel.ExecuteLogin(txtPassword.Password);
            }
        }

        // Обработчик изменения пароля
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is AuthViewModel viewModel)
            {
                viewModel.Password = txtPassword.Password;
            }
        }
    }
}