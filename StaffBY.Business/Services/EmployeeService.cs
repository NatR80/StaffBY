using Microsoft.EntityFrameworkCore;
using StaffBY.Business.Interfaces;
using StaffBY.DAL.Context;
using StaffBY.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffBY.Business.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly StaffBYDbContext _context;

        public EmployeeService(StaffBYDbContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Department)
                .Where(e => !e.IsArchived)
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ToListAsync();
        }

        public async Task<List<Employee>> GetAllEmployeesWithArchivedAsync()
        {
            return await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Department)
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Employee?> GetEmployeeByPersonalNumberAsync(string personalNumber)
        {
            return await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.PersonalNumber == personalNumber);
        }

        public async Task<List<Employee>> SearchEmployeesAsync(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return await GetAllEmployeesAsync();

            return await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Department)
                .Where(e => !e.IsArchived &&
                    (e.LastName.Contains(searchText) ||
                     e.FirstName.Contains(searchText) ||
                     (e.Patronymic != null && e.Patronymic.Contains(searchText)) ||
                     e.PersonalNumber.Contains(searchText)))
                .OrderBy(e => e.LastName)
                .ToListAsync();
        }

        public async Task<Employee> AddEmployeeAsync(Employee employee)
        {
            if (string.IsNullOrEmpty(employee.PersonalNumber))
            {
                employee.PersonalNumber = GeneratePersonalNumber();
            }

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee employee)
        {
            var existing = await _context.Employees.FindAsync(employee.Id);
            if (existing == null)
                throw new Exception("Сотрудник не найден");

            // Обновляем только существующие поля
            existing.LastName = employee.LastName;
            existing.FirstName = employee.FirstName;
            existing.Patronymic = employee.Patronymic;
            existing.PersonalNumber = employee.PersonalNumber;
            existing.BirthDate = employee.BirthDate;
            existing.Phone = employee.Phone;
            existing.Email = employee.Email;
            existing.HomeAddress = employee.HomeAddress;
            existing.PositionId = employee.PositionId;
            existing.DepartmentId = employee.DepartmentId;
            //existing.HireDate = employee.HireDate;
            //existing.Salary = employee.Salary;
            existing.IsArchived = employee.IsArchived;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DismissEmployeeAsync(int id, DateTime dismissalDate, string reason)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return false;

            employee.IsArchived = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return false;

            employee.IsArchived = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return false;

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Employee>> GetEmployeesByBirthdayAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Employees
                .Where(e => !e.IsArchived &&
                    e.BirthDate.Month >= startDate.Month && e.BirthDate.Month <= endDate.Month)
                .OrderBy(e => e.BirthDate.Month)
                .ThenBy(e => e.BirthDate.Day)
                .ToListAsync();
        }

        public async Task<EmployeeStatistics> GetStatisticsAsync()
        {
            var allEmployees = await _context.Employees.ToListAsync();
            var activeEmployees = allEmployees.Where(e => !e.IsArchived).ToList();

            return new EmployeeStatistics
            {
                TotalCount = allEmployees.Count,
                ActiveCount = activeEmployees.Count,
                ArchivedCount = allEmployees.Count(e => e.IsArchived),
                MenCount = 0,  // Временно, пока нет поля Gender
                WomenCount = 0, // Временно, пока нет поля Gender
                AverageAge = activeEmployees.Any() ?
                    (int)activeEmployees.Average(e => DateTime.Now.Year - e.BirthDate.Year) : 0
            };
        }

        private string GeneratePersonalNumber()
        {
            string prefix = DateTime.Now.ToString("yyMMdd");
            var lastEmployee = _context.Employees
                .Where(e => e.PersonalNumber != null && e.PersonalNumber.StartsWith(prefix))
                .OrderByDescending(e => e.PersonalNumber)
                .FirstOrDefault();

            int number = 1;
            if (lastEmployee != null && lastEmployee.PersonalNumber != null && lastEmployee.PersonalNumber.Length >= 10)
            {
                if (int.TryParse(lastEmployee.PersonalNumber.Substring(6, 4), out int lastNum))
                    number = lastNum + 1;
            }

            return $"{prefix}{number:D4}";
        }
    }
}