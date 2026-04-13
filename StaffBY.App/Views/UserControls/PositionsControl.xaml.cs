using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StaffBY.App.ViewModels;
using StaffBY.App.ViewModels.StaffBY.App.ViewModels;
using StaffBY.App.Views;

namespace StaffBY.App.Views.UserControls
{
    public partial class PositionsControl : UserControl
    {
        private List<PositionViewModel> _positions = new List<PositionViewModel>();  // ← ИСПРАВЛЕНО
        private List<string> _departments = new List<string>();
        public event Action<string>? StatusMessageChanged;

        public PositionsControl()
        {
            InitializeComponent();
            LoadDemoData();
        }

        // ИСПРАВЛЕНО: возвращаем List<PositionViewModel>
        public List<PositionViewModel> GetPositions()
        {
            return _positions;
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

            // ИСПРАВЛЕНО: используем PositionViewModel
            _positions = new List<PositionViewModel>
            {
                new PositionViewModel
                {
                    Id = 1,
                    Name = "Директор",
                    DepartmentName = "Руководство",
                    NumberOfPositions = 1,
                    FilledPositions = 1,
                    Rate = 1.0m,
                    WorkWeek = "5-дневная",
                    Category = "Руководитель",
                    Salary = 5000,
                    Allowance = 1000
                },
                new PositionViewModel
                {
                    Id = 2,
                    Name = "Главный бухгалтер",
                    DepartmentName = "Бухгалтерия",
                    NumberOfPositions = 1,
                    FilledPositions = 1,
                    Rate = 1.0m,
                    WorkWeek = "5-дневная",
                    Category = "Руководитель",
                    Salary = 3500,
                    Allowance = 500
                },
                new PositionViewModel
                {
                    Id = 3,
                    Name = "Инженер-программист",
                    DepartmentName = "IT отдел",
                    NumberOfPositions = 3,
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
            var deptsFromPositions = _positions.Select(p => p.DepartmentName).Distinct();
            _departments = _departments.Union(deptsFromPositions).OrderBy(d => d).ToList();
        }

        private void AddDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
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
            RefreshDepartmentsList();

            var dialog = new PositionEditWindow(null, _departments);
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true && dialog.Result != null)
            {
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
            if (PositionsDataGrid.SelectedItem is PositionViewModel selected)
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
            if (PositionsDataGrid.SelectedItem is PositionViewModel selected)
            {
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

        public List<string> GetDepartments()
        {
            RefreshDepartmentsList();
            return _departments;
        }
    }
}