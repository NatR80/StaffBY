using StaffBY.Business.Services;
using StaffBY.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace StaffBY.Business.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetAllEmployeesAsync();
        Task<List<Employee>> GetAllEmployeesWithArchivedAsync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<Employee?> GetEmployeeByPersonalNumberAsync(string personalNumber);
        Task<List<Employee>> SearchEmployeesAsync(string searchText);
        Task<Employee> AddEmployeeAsync(Employee employee);
        Task<Employee> UpdateEmployeeAsync(Employee employee);
        Task<bool> DismissEmployeeAsync(int id, DateTime dismissalDate, string reason);
        Task<bool> RestoreEmployeeAsync(int id);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<List<Employee>> GetEmployeesByBirthdayAsync(DateTime startDate, DateTime endDate);
        Task<EmployeeStatistics> GetStatisticsAsync();
    }

    public class EmployeeStatistics
    {
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public int ArchivedCount { get; set; }
        public int MenCount { get; set; }
        public int WomenCount { get; set; }
        public int AverageAge { get; set; }
    }
}

