using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StaffBY.App.Models;
using StaffBY.App.Views;

namespace StaffBY.App.Views.UserControls
{
    public partial class PositionsControl : UserControl
    {
        private List<PositionInStaffModel> _positions = new List<PositionInStaffModel>();
        private List<string> _departments = new List<string>();
        public event Action<string>? StatusMessageChanged;

        public PositionsControl()
        {
            InitializeComponent();
            LoadDemoData();
        }

        private void LoadDemoData()
        {
            // Начальный список подразделений
            _departments = new List<string>
            {
                "Руководство",
                "Бухгалтерия",
                "IT отдел",
                "Отдел продаж",
                "Производственный отдел",
                "Отдел кадров"
            };

            _positions = new List<PositionInStaffModel>
            {
                new PositionInStaffModel
                {
                    Id = 1,
                    Name = "Директор",
                    DepartmentName = "Руководство",
                    StaffUnits = 1,
                    FilledPositions = 1,
                    Rate = 1.0m,
                    WorkWeek = "5-дневная",
                    Category = "Руководитель",
                    Salary = 5000,
                    Allowance = 1000
                },
                new PositionInStaffModel
                {
                    Id = 2,
                    Name = "Главный бухгалтер",
                    DepartmentName = "Бухгалтерия",
                    StaffUnits = 1,
                    FilledPositions = 1,
                    Rate = 1.0m,
                    WorkWeek = "5-дневная",
                    Category = "Руководитель",
                    Salary = 3500,
                    Allowance = 500
                },
                new PositionInStaffModel
                {
                    Id = 3,
                    Name = "Инженер-программист",
                    DepartmentName = "IT отдел",
                    StaffUnits = 3,
                    FilledPositions = 2,
                    Rate = 1.0m,
                    WorkWeek = "5-дневная",
                    Category = "Специалист",
                    Salary = 2500,
                    Allowance = 300
                }
            };
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            PositionsDataGrid.ItemsSource = null;
            PositionsDataGrid.ItemsSource = _positions;
        }

        private void RefreshDepartmentsList()
        {
            // Обновляем список подразделений из должностей
            var deptsFromPositions = _positions.Select(p => p.DepartmentName).Distinct();
            // Объединяем с существующими подразделениями и удаляем дубликаты
            _departments = _departments.Union(deptsFromPositions).OrderBy(d => d).ToList();
        }

        /// <summary>
        /// Добавление нового подразделения (без отдельного окна)
        /// </summary>
        private void AddDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            // Используем стандартное окно ввода от Microsoft.VisualBasic
            string newDepartment = Microsoft.VisualBasic.Interaction.InputBox(
                "Введите название нового подразделения:",
                "Добавление подразделения",
                "");

            if (!string.IsNullOrWhiteSpace(newDepartment))
            {
                newDepartment = newDepartment.Trim();

                if (!_departments.Contains(newDepartment))
                {
                    _departments.Add(newDepartment);
                    _departments = _departments.OrderBy(d => d).ToList();
                    StatusMessageChanged?.Invoke($"Подразделение '{newDepartment}' добавлено");
                    MessageBox.Show($"Подразделение '{newDepartment}' добавлено", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Подразделение '{newDepartment}' уже существует", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void AddPositionButton_Click(object sender, RoutedEventArgs e)
        {
            // Обновляем список подразделений перед открытием диалога
            RefreshDepartmentsList();

            var dialog = new PositionEditWindow(null, _departments);
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true && dialog.Result != null)
            {
                // Генерируем новый Id
                int newId = _positions.Count > 0 ? _positions.Max(p => p.Id) + 1 : 1;
                dialog.Result.Id = newId;
                dialog.Result.FilledPositions = 0;
                _positions.Add(dialog.Result);
                RefreshGrid();
                RefreshDepartmentsList();
                StatusMessageChanged?.Invoke($"Должность '{dialog.Result.Name}' добавлена");
            }
        }

        private void EditPositionButton_Click(object sender, RoutedEventArgs e)
        {
            if (PositionsDataGrid.SelectedItem is PositionInStaffModel selected)
            {
                RefreshDepartmentsList();

                var dialog = new PositionEditWindow(selected, _departments);
                dialog.Owner = Window.GetWindow(this);

                if (dialog.ShowDialog() == true && dialog.Result != null)
                {
                    var index = _positions.FindIndex(p => p.Id == selected.Id);
                    if (index >= 0)
                    {
                        _positions[index] = dialog.Result;
                        RefreshGrid();
                        RefreshDepartmentsList();
                        StatusMessageChanged?.Invoke($"Должность '{dialog.Result.Name}' обновлена");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите должность для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeletePositionButton_Click(object sender, RoutedEventArgs e)
        {
            if (PositionsDataGrid.SelectedItem is PositionInStaffModel selected)
            {
                // Проверка: есть ли сотрудники на этой должности?
                if (selected.FilledPositions > 0)
                {
                    MessageBox.Show($"Нельзя удалить должность '{selected.Name}', так как на ней есть сотрудники ({selected.FilledPositions} чел.)",
                        "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show($"Удалить должность '{selected.Name}'?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _positions.Remove(selected);
                    RefreshGrid();
                    RefreshDepartmentsList();
                    StatusMessageChanged?.Invoke($"Должность '{selected.Name}' удалена");
                }
            }
            else
            {
                MessageBox.Show("Выберите должность для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Получить список подразделений (для других контролов)
        /// </summary>
        public List<string> GetDepartments()
        {
            RefreshDepartmentsList();
            return _departments;
        }

        /// <summary>
        /// Получить список должностей (для других контролов)
        /// </summary>
        public List<PositionInStaffModel> GetPositions()
        {
            return _positions;
        }
    }
}