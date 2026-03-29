using System;
using System.Collections.Generic;

namespace StaffBY.App.ViewModels
{
    /// <summary>
    /// Модель организации
    /// </summary>
    public class OrganizationViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string UNP { get; set; } = string.Empty;
        public string LegalAddress { get; set; } = string.Empty;
        public int? DirectorId { get; set; }
        public string DirectorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Модель табеля учета рабочего времени
    /// </summary>
    public class TimesheetViewModel
    {
        public int EmployeeId { get; set; }
        public string PersonalNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int WorkedDays { get; set; }      // Отработано дней
        public int SickDays { get; set; }        // Больничные
        public int VacationDays { get; set; }    // Отпуск
        public int AbsentDays { get; set; }      // Прогулы/неявки
    }

    /// <summary>
    /// Модель начисления зарплаты
    /// </summary>
    public class PayrollAccrualViewModel
    {
        public int EmployeeId { get; set; }
        public string PersonalNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;
        public decimal Salary { get; set; }          // Оклад
        public int WorkedDays { get; set; }          // Отработано дней
        public decimal AccruedAmount { get; set; }   // Начислено
    }

    /// <summary>
    /// Модель выплаты зарплаты (с налогами)
    /// </summary>
    public class PayrollPaymentViewModel
    {
        public int EmployeeId { get; set; }
        public string PersonalNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public decimal AccruedAmount { get; set; }      // Начислено
        public decimal IncomeTax { get; set; }           // Подоходный налог 13%
        public decimal Contributions { get; set; }       // Взносы 1%
        public decimal NetAmount { get; set; }           // К выплате
        public List<DeductionDetail>? Deductions { get; set; }  // Детали удержаний
    }

    /// <summary>
    /// Детали удержаний
    /// </summary>
    public class DeductionDetail
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}