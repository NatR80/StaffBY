using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StaffBY.Business.Interfaces;
using StaffBY.Business.Services;
using StaffBY.DAL.Configuration;
using StaffBY.DAL.Context;
using StaffBY.App.ViewModels;
using StaffBY.App.Views;
using System.Windows;

namespace StaffBY.App
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        // ДОБАВЬТЕ ЭТО СВОЙСТВО
        public IServiceProvider Services => _serviceProvider;

        private void ConfigureServices(IServiceCollection services)
        {
            var connectionString = DatabaseConfig.GetConnectionString();
            services.AddDbContext<StaffBYDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmployeeService, EmployeeService>();

            services.AddScoped<AuthViewModel>();

            services.AddTransient<AuthWindow>();
            services.AddTransient<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var authWindow = _serviceProvider.GetRequiredService<AuthWindow>();
                var authViewModel = _serviceProvider.GetRequiredService<AuthViewModel>();
                authWindow.DataContext = authViewModel;

                authWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при запуске приложения: {ex.Message}",
                    "Ошибка запуска",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}