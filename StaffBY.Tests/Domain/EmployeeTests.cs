using StaffBY.Domain.Entities;
using Xunit;

namespace StaffBY.Tests.Domain
{
    public class EmployeeTests
    {
        [Fact]
        public void GetFullName_WithPatronymic_ReturnsFullName()
        {
            // Arrange - готовим данные
            var employee = new Employee
            {
                LastName = "Иванов",
                FirstName = "Иван",
                Patronymic = "Иванович"
            };

            // Act - выполняем метод
            var result = employee.GetFullName();

            // Assert - проверяем результат
            Assert.Equal("Иванов Иван Иванович", result);
        }

        [Fact]
        public void GetFullName_WithoutPatronymic_ReturnsNameWithoutPatronymic()
        {
            // Arrange
            var employee = new Employee
            {
                LastName = "Петрова",
                FirstName = "Анна",
                Patronymic = null
            };

            // Act
            var result = employee.GetFullName();

            // Assert
            Assert.Equal("Петрова Анна", result);
        }

        [Fact]
        public void GetShortName_WithPatronymic_ReturnsInitials()
        {
            // Arrange
            var employee = new Employee
            {
                LastName = "Сидоров",
                FirstName = "Сергей",
                Patronymic = "Петрович"
            };

            // Act
            var result = employee.GetShortName();

            // Assert
            Assert.Equal("Сидоров С.П.", result);
        }

        [Fact]
        public void GetAge_ReturnsCorrectAge()
        {
            // Arrange - создаем сотрудника с датой рождения 20 лет назад
            var birthDate = DateTime.Today.AddYears(-20);
            var employee = new Employee
            {
                BirthDate = birthDate
            };

            // Act
            var age = employee.GetAge();

            // Assert
            Assert.Equal(20, age);
        }
    }
}