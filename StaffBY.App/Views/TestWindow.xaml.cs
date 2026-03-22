using Microsoft.EntityFrameworkCore;
using StaffBY.Business.Services;
using StaffBY.DAL.Configuration;
using StaffBY.DAL.Context;
using System.Windows;

namespace StaffBY.App.Views
{
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Проверяем строку подключения
                var connectionString = DatabaseConfig.GetConnectionString();
                MessageBox.Show($"Строка подключения: {connectionString}");

                // 2. Создаем контекст
                var options = new DbContextOptionsBuilder<StaffBYDbContext>()
                    .UseSqlServer(connectionString)
                    .Options;

                using var context = new StaffBYDbContext(options);

                // 3. Проверяем, подключились ли к БД
                var canConnect = await context.Database.CanConnectAsync();
                MessageBox.Show($"Подключение к БД: {(canConnect ? "Да" : "Нет")}");

                if (!canConnect)
                {
                    txtResult.Text = "Не удалось подключиться к базе данных";
                    return;
                }

                // 4. Считаем пользователей
                var allUsers = await context.Users.ToListAsync();
                MessageBox.Show($"Всего пользователей в БД: {allUsers.Count}");

                if (allUsers.Count == 0)
                {
                    txtResult.Text = "В базе данных нет пользователей!";
                    return;
                }

                // 5. Показываем всех пользователей
                string usersList = "";
                foreach (var u in allUsers)
                {
                    usersList += $"{u.Username} (Роль: {u.Role})\n";
                }
                MessageBox.Show($"Пользователи в БД:\n{usersList}");

                // 6. Пытаемся авторизоваться
                var userService = new UserService(context);
                var user = await userService.AuthenticateAsync(txtLogin.Text, txtPassword.Password);

                if (user != null)
                {
                    txtResult.Text = $"Успешно! Пользователь: {user.Username}, Роль: {user.Role}";
                    txtResult.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
                }
                else
                {
                    txtResult.Text = "Неверный логин или пароль";
                }
            }
            catch (Exception ex)
            {
                txtResult.Text = $"Ошибка: {ex.Message}";
                MessageBox.Show($"Детали ошибки: {ex.ToString()}");
            }
        }
    }
}