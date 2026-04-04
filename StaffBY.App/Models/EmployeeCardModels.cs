using System;
using System.Collections.Generic;
using StaffBY.App.Models;

namespace StaffBY.App.Models
{
    /// <summary>
    /// Класс для хранения записи об отпуске
    /// </summary>
    public class VacationEntry
    {
        public int Id { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int BasicDays { get; set; } = 24;
        public int AdditionalDays { get; set; }
        public int TotalDays => BasicDays + AdditionalDays;
        public int UsedDays { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? AvailableFrom { get; set; }  // Предоставляется отпуск с
        public int RemainingDays => TotalDays - UsedDays;
        public string Basis { get; set; } = string.Empty;
        public string Schedule { get; set; } = string.Empty;
        public string PeriodName => $"{PeriodStart:yyyy} - {PeriodEnd:yyyy}";

    }


    /// <summary>
    /// Класс для хранения члена семьи
    /// </summary>
    public class FamilyMemberEntry
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;

        private DateTime _birthDate;
        public DateTime BirthDate
        {
            get => _birthDate;
            set
            {
                _birthDate = value;
                BirthYear = value.Year;
            }
        }

        public int BirthYear { get; private set; }
        public string WorkPlace { get; set; } = string.Empty; // Место работы, учебы
        public string Document { get; set; } = string.Empty; // Документ (свидетельство о рождении, паспорт, справка)

        // Для обратной совместимости с существующим кодом
        public string FullInfo => $"{FullName} ({Relationship})";

        // Для отображения даты рождения в строковом формате
        public string BirthDateString => BirthDate == DateTime.MinValue ? "" : BirthDate.ToString("dd.MM.yyyy");
    }



}
