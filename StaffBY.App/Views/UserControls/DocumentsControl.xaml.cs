using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StaffBY.App.ViewModels;

namespace StaffBY.App.Views.UserControls
{
    /// <summary>
    /// Контрол для генерации кадровых документов
    /// Позволяет создавать трудовые договоры, приказы о приеме, увольнении и отпуске
    /// </summary>
    public partial class DocumentsControl : UserControl
    {
        // Событие для отправки сообщений в статусную строку главного окна
        public event Action<string>? StatusMessageChanged;

        // Список всех сотрудников (полученный из EmployeesControl)
        private List<EmployeeViewModel> _employees = new List<EmployeeViewModel>();

        public DocumentsControl()
        {
            InitializeComponent();

            // Инициализируем выпадающие списки
            InitializeComboBoxes();
        }

        /// <summary>
        /// Инициализирует выпадающие списки типов документов
        /// </summary>
        private void InitializeComboBoxes()
        {
            // Проверяем, что ComboBox существуют в XAML
            if (cmbDocumentType != null)
            {
                cmbDocumentType.Items.Clear();
                cmbDocumentType.Items.Add(new ComboBoxItem { Content = "Трудовой договор" });
                cmbDocumentType.Items.Add(new ComboBoxItem { Content = "Приказ о приеме" });
                cmbDocumentType.Items.Add(new ComboBoxItem { Content = "Приказ об увольнении" });
                cmbDocumentType.Items.Add(new ComboBoxItem { Content = "Приказ об отпуске" });
                cmbDocumentType.SelectedIndex = 0; // Выбираем первый пункт по умолчанию
            }
        }

        /// <summary>
        /// Устанавливает список сотрудников (вызывается из MainWindow)
        /// </summary>
        /// <param name="employees">Список сотрудников из EmployeesControl</param>
        public void SetEmployees(List<EmployeeViewModel> employees)
        {
            _employees = employees ?? new List<EmployeeViewModel>();

            // Заполняем ComboBox сотрудников только активными (не уволенными)
            var activeEmployees = _employees.Where(e => !e.IsArchived).ToList();

            if (cmbEmployee != null)
            {
                cmbEmployee.ItemsSource = activeEmployees;
                cmbEmployee.DisplayMemberPath = "FullName"; // Показываем ФИО
                cmbEmployee.SelectedValuePath = "Id";      // Значение - ID
            }

            StatusMessageChanged?.Invoke($"Загружено {activeEmployees.Count} сотрудников для документов");
        }

        /// <summary>
        /// Обработчик кнопки "Сформировать документ"
        /// </summary>
        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбран ли сотрудник
            if (cmbEmployee?.SelectedItem == null)
            {
                StatusMessageChanged?.Invoke("Выберите сотрудника");
                MessageBox.Show("Выберите сотрудника из списка", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Получаем выбранного сотрудника
            var employee = cmbEmployee.SelectedItem as EmployeeViewModel;
            if (employee == null)
            {
                StatusMessageChanged?.Invoke("Ошибка: сотрудник не найден");
                return;
            }

            // Получаем тип документа
            string docType = "";
            if (cmbDocumentType?.SelectedItem is ComboBoxItem selectedItem)
            {
                docType = selectedItem.Content?.ToString() ?? "";
            }

            if (string.IsNullOrEmpty(docType))
            {
                StatusMessageChanged?.Invoke("Выберите тип документа");
                return;
            }

            // Генерируем документ
            string document = GenerateDocument(docType, employee);

            // Отображаем в текстовом поле
            if (txtPreview != null)
            {
                txtPreview.Text = document;
            }

            StatusMessageChanged?.Invoke($"Документ '{docType}' сформирован для {employee.FullName}");
        }

        /// <summary>
        /// Генерирует текст документа на основе типа и данных сотрудника
        /// </summary>
        /// <param name="docType">Тип документа</param>
        /// <param name="emp">Данные сотрудника</param>
        /// <returns>Текст документа</returns>
        private string GenerateDocument(string docType, EmployeeViewModel emp)
        {
            string date = DateTime.Now.ToString("dd.MM.yyyy");
            string currentYear = DateTime.Now.Year.ToString();

            // Формируем полное ФИО для документов
            string fullName = $"{emp.LastName} {emp.FirstName} {emp.Patronymic}".Trim();

            switch (docType)
            {
                case "Трудовой договор":
                    return GenerateEmploymentContract(emp, fullName, date);

                case "Приказ о приеме":
                    return GenerateHireOrder(emp, fullName, date);

                case "Приказ об увольнении":
                    return GenerateTerminationOrder(emp, fullName, date);

                case "Приказ об отпуске":
                    return GenerateVacationOrder(emp, fullName, date);

                default:
                    return "Выберите тип документа из списка";
            }
        }

        /// <summary>
        /// Генерирует трудовой договор
        /// </summary>
        private string GenerateEmploymentContract(EmployeeViewModel emp, string fullName, string date)
        {
            return $"ТРУДОВОЙ ДОГОВОР (КОНТРАКТ) № ______\n\n" +
                   $"г. Минск                     {date}\n\n" +
                   $"─────────────────────────────────────────────────\n\n" +
                   $"ООО \"Наименование организации\", именуемое в дальнейшем «Наниматель»,\n" +
                   $"в лице директора _______________________, действующего на основании Устава,\n" +
                   $"с одной стороны, и\n\n" +
                   $"Гражданин(ка) {fullName}\n" +
                   $"Паспорт: ______ № ______ выдан ______________________\n" +
                   $"Адрес: ________________________________________________\n" +
                   $"именуемый(ая) в дальнейшем «Работник», с другой стороны,\n" +
                   $"заключили настоящий договор о следующем:\n\n" +
                   $"─────────────────────────────────────────────────\n\n" +
                   $"1. ПРЕДМЕТ ДОГОВОРА\n\n" +
                   $"1.1. Наниматель предоставляет Работнику работу по должности:\n" +
                   $"     {emp.PositionName}\n\n" +
                   $"1.2. Работник обязуется лично выполнять работу в соответствии с условиями\n" +
                   $"     настоящего договора и соблюдать Правила внутреннего трудового распорядка.\n\n" +
                   $"2. ОПЛАТА ТРУДА\n\n" +
                   $"2.1. За выполнение работы Работнику устанавливается должностной оклад\n" +
                   $"     в размере {emp.Salary:N2} (Сорок тысяч) белорусских рублей в месяц.\n\n" +
                   $"2.2. Выплата заработной платы производится 2 раза в месяц.\n\n" +
                   $"3. РЕЖИМ РАБОЧЕГО ВРЕМЕНИ И ОТДЫХА\n\n" +
                   $"3.1. Работнику устанавливается 5-дневная рабочая неделя с выходными днями\n" +
                   $"     в субботу и воскресенье.\n\n" +
                   $"4. СРОК ДЕЙСТВИЯ ДОГОВОРА\n\n" +
                   $"4.1. Договор вступает в силу с {DateTime.Now.AddDays(7):dd.MM.yyyy} и действует до ________.\n\n" +
                   $"─────────────────────────────────────────────────\n\n" +
                   $"ПОДПИСИ СТОРОН:\n\n" +
                   $"Наниматель: _________________    Работник: _________________\n\n" +
                   $"М.П.\n\n" +
                   $"Дата: {date}";
        }

        /// <summary>
        /// Генерирует приказ о приеме на работу
        /// </summary>
        private string GenerateHireOrder(EmployeeViewModel emp, string fullName, string date)
        {
            return $"ОБЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ \"НАИМЕНОВАНИЕ\"\n\n" +
                   $"                           ПРИКАЗ № ______\n" +
                   $"                           {date}\n\n" +
                   $"─────────────────────────────────────────────────\n\n" +
                   $"                         О ПРИЕМЕ НА РАБОТУ\n\n" +
                   $"─────────────────────────────────────────────────\n\n" +
                   $"ПРИНЯТЬ НА РАБОТУ:\n\n" +
                   $"Фамилия, имя, отчество: {fullName}\n" +
                   $"Должность: {emp.PositionName}\n" +
                   $"Подразделение: {emp.DepartmentName}\n" +
                   $"Дата начала работы: {DateTime.Now.AddDays(7):dd.MM.yyyy}\n" +
                   $"Оклад: {emp.Salary:N2} рублей\n" +
                   $"Условия приема: Постоянно\n" +
                   $"Характер работы: Постоянная\n" +
                   $"Испытательный срок: 3 месяца\n\n" +
                   $"─────────────────────────────────────────────────\n\n" +
                   $"ОСНОВАНИЕ: трудовой договор № ______ от {date}\n\n" +
                   $"─────────────────────────────────────────────────\n\n" +
                   $"Директор: ______________________\n\n" +
                   $"С приказом ознакомлен(а): ______________________\n" +
                   $"Дата ознакомления: {date}";
        }

        /// <summary>
        /// Генерирует приказ об увольнении
        /// </summary>
        private string GenerateTerminationOrder(EmployeeViewModel emp, string fullName, string date)
        {
            return $"ОБЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ \"НАИМЕНОВАНИЕ\"\n\n" +
                   $"                           ПРИКАЗ № ______\n" +
                   $"                           {date}\n\n" +
                   $"─────────────────────────────────────────────────\n\n" +
                   $"                        О ПРЕКРАЩЕНИИ ТРУДОВОГО ДОГОВОРА\n\n" +
                   $"─────────────────────────────────────────────────\n\n" +
                   $"УВОЛИТЬ:\n\n" +
                   $"Фамилия, имя, отчество: {fullName}\n" +
                   $"Должность: {emp.PositionName}\n" +
                   $"Подразделение: {emp.DepartmentName}\n" +
                   $"Дата увольнения: {DateTime.Now.AddDays(14):dd.MM.yyyy}\n" +
                   $"Причина увольнения: По собственному желанию (статья 40 Трудового кодекса РБ)\n\n" +
                   $"─────────────────────────────────────────────────\n\n" +
                   $"ОСНОВАНИЕ: заявление работника от {date}\n\n" +
                   $"─────────────────────────────────────────────────\n\n" +
                   $"Директор: ______________________\n\n" +
                   $"С приказом ознакомлен(а): ______________________\n" +
                   $"Дата ознакомления: {date}";
        }

        /// <summary>
        /// Генерирует приказ об отпуске
        /// </summary>
        private string GenerateVacationOrder(EmployeeViewModel emp, string fullName, string date)
        {
            DateTime vacationStart = DateTime.Now.AddDays(7);
            DateTime vacationEnd = vacationStart.AddDays(23); // 24 дня минус 1

            return $"ОБЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ \"НАИМЕНОВАНИЕ\"\n\n" +
                   $"                           ПРИКАЗ № ______\n" +
                   $"                           {date}\n\n" +
                   $"─────────────────────────────────────────────────\n\n" +
                   $"                         О ПРЕДОСТАВЛЕНИИ ОТПУСКА\n\n" +
                   $"─────────────────────────────────────────────────\n\n" +
                   $"ПРЕДОСТАВИТЬ ОТПУСК:\n\n" +
                   $"Фамилия, имя, отчество: {fullName}\n" +
                   $"Должность: {emp.PositionName}\n" +
                   $"Подразделение: {emp.DepartmentName}\n\n" +
                   $"Вид отпуска: Основной трудовой отпуск\n" +
                   $"За период работы: с {DateTime.Now.AddYears(-1):dd.MM.yyyy} по {DateTime.Now:dd.MM.yyyy}\n" +
                   $"Количество календарных дней: 24\n" +
                   $"Дата начала отпуска: {vacationStart:dd.MM.yyyy}\n" +
                   $"Дата окончания отпуска: {vacationEnd:dd.MM.yyyy}\n\n" +
                   $"─────────────────────────────────────────────────\n\n" +
                   $"ОСНОВАНИЕ: график отпусков, заявление работника\n\n" +
                   $"─────────────────────────────────────────────────\n\n" +
                   $"Директор: ______________________\n\n" +
                   $"С приказом ознакомлен(а): ______________________\n" +
                   $"Дата ознакомления: {date}";
        }

        /// <summary>
        /// Обработчик кнопки "Экспорт в Word"
        /// </summary>
        private void ExportToWordButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPreview?.Text) ||
                txtPreview.Text == "Выберите тип документа из списка")
            {
                StatusMessageChanged?.Invoke("Сначала сформируйте документ");
                MessageBox.Show("Сначала сформируйте документ", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Здесь будет логика экспорта в Word
            StatusMessageChanged?.Invoke("Экспорт в Word... (функция в разработке)");
            MessageBox.Show("Экспорт в Word будет добавлен в следующей версии",
                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Обработчик кнопки "Печать"
        /// </summary>
        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPreview?.Text) ||
                txtPreview.Text == "Выберите тип документа из списка")
            {
                StatusMessageChanged?.Invoke("Сначала сформируйте документ");
                MessageBox.Show("Сначала сформируйте документ", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Здесь будет логика печати
            StatusMessageChanged?.Invoke("Отправка документа на печать...");
            MessageBox.Show("Печать документа будет добавлена в следующей версии",
                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}