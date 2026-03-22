using StaffBY.Business.Interfaces;
using StaffBY.Domain.Entities;
using StaffBY.App.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace StaffBY.App.ViewModels
{
    public class AuthViewModel : INotifyPropertyChanged
    {
        private readonly IUserService? _userService;
        private string _username = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoading;
        private string _password = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public AuthViewModel()
        {
        }

        public AuthViewModel(IUserService userService)
        {
            _userService = userService;
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
                ClearError();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                ClearError();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private void ClearError()
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
                ErrorMessage = string.Empty;
        }

        public ICommand LoginCommand => new RelayCommand(ExecuteLogin);

        /// <summary>
        /// Метод для входа - ВСЯ ЛОГИКА АУТЕНТИФИКАЦИИ ЗДЕСЬ
        /// </summary>
        public async void ExecuteLogin(object? parameter)
        {
            // Получаем пароль из параметра или из свойства
            string password = parameter as string ?? Password;

            // Проверка на пустой пароль
            if (string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Введите пароль";
                return;
            }

            // Проверка на пустой логин
            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Введите логин";
                return;
            }

            if (_userService == null)
            {
                ErrorMessage = "Сервис не инициализирован";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                // Вызываем сервис аутентификации
                var user = await _userService.AuthenticateAsync(Username, password);

                if (user != null)
                {
                    // УСПЕШНЫЙ ВХОД - открываем главное окно
                    var mainWindow = new MainWindow(user);
                    mainWindow.Show();

                    // Закрываем окно авторизации
                    Application.Current.Windows.OfType<AuthWindow>().FirstOrDefault()?.Close();
                }
                else
                {
                    ErrorMessage = "Неверный логин или пароль";
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}