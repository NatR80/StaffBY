using System;
using StaffBY.App.Models;
using System.Collections.Generic;


namespace StaffBY.App.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public string PersonalNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public string FullName => $"{LastName} {FirstName} {Patronymic}".Trim();
        

        // Общие сведения
        public DateTime BirthDate { get; set; }
        public string BirthPlace { get; set; } = string.Empty;
        public string Citizenship { get; set; } = string.Empty;
        public string Education { get; set; } = string.Empty;
        public string EducationInstitution { get; set; } = string.Empty;
        public DateTime? EducationEndDate { get; set; }
        public string Qualification { get; set; } = string.Empty;
        

        // Паспортные данные
        public string PassportSeries { get; set; } = string.Empty;
        public string PassportNumber { get; set; } = string.Empty;
        public DateTime? PassportIssueDate { get; set; }
        public string PassportIssuedBy { get; set; } = string.Empty;
        public DateTime? PassportExpiryDate { get; set; }
        public string IdentificationNumber { get; set; } = string.Empty;
        public string InsuranceNumber { get; set; } = string.Empty;

        // Контакты и адрес
        public string MaritalStatus { get; set; } = string.Empty;
        public string HomeAddress { get; set; } = string.Empty;
        public string RegistrationAddress { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Трудоустройство
        public int? PositionId { get; set; }
        public string PositionName { get; set; } = string.Empty;
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public DateTime? HireDate { get; set; }
        public decimal? Salary { get; set; }
        public string EmploymentType { get; set; } = string.Empty;
        public string ContractType { get; set; } = string.Empty;
        public DateTime? ContractEndDate { get; set; }

        // Воинский учет
        public string MilitaryCategory { get; set; } = string.Empty;
        public string MilitaryComposition { get; set; } = string.Empty;
        public string MilitaryRank { get; set; } = string.Empty;
        public string MilitaryVUS { get; set; } = string.Empty;
        public string MilitaryFitness { get; set; } = string.Empty;
        public string MilitaryCommissariat { get; set; } = string.Empty;
        public bool IsSpecialRegistration { get; set; }
        public string MobilizationNumber { get; set; } = string.Empty;
        public DateTime? RegistrationDate { get; set; }
        public bool IsRemovedFromRegistry { get; set; }
        public DateTime? RemovalDate { get; set; }

        // Статус
        public bool IsArchived { get; set; }
        public DateTime? DismissalDate { get; set; }
        public string DismissalReason { get; set; } = string.Empty;

        // Дополнительная информация
        public string AdditionalInfo { get; set; } = string.Empty;

        

        // Для дополнительных отпусков (если нужно сохранять)
        public int ContractVacationDays { get; set; }      // За контракт
        public int HarmfulVacationDays { get; set; }       // За вредность
        public int IrregularVacationDays { get; set; }     // За ненормированный день
        public int ExperienceVacationDays { get; set; }    // За стаж
        public int BonusVacationDays { get; set; }         // Поощрительный

        
        /// Количество детей и иждивенцев (для ручного ввода кадровиком)        
        public int DependentsCount { get; set; }
        // Для членов семьи

        public List<FamilyMemberEntry> FamilyMembers { get; set; } = new List<FamilyMemberEntry>();



    }



}