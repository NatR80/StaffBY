using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using StaffBY.App.Models;
using StaffBY.App.Views;

namespace StaffBY.App.Views.UserControls.EmployeeCard
{
    public partial class FamilyControl : UserControl
    {
        private List<FamilyMemberEntry> _familyMembers = new();

        public FamilyControl()
        {
            InitializeComponent();
        }

        public void LoadData(List<FamilyMemberEntry> familyMembers)
        {
            _familyMembers = familyMembers ?? new List<FamilyMemberEntry>();
            dgFamilyMembers.ItemsSource = _familyMembers;
        }

        public List<FamilyMemberEntry> GetFamilyMembers() => _familyMembers;

        private void AddFamilyMember_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FamilyMemberEditWindow();
            dialog.MemberSaved += (s, member) =>
            {
                _familyMembers.Add(member);
                RefreshGrid();
            };
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        private void RemoveFamilyMember_Click(object sender, RoutedEventArgs e)
        {
            if (dgFamilyMembers.SelectedItem is FamilyMemberEntry selected)
            {
                var result = MessageBox.Show($"Удалить члена семьи '{selected.FullName}'?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _familyMembers.Remove(selected);
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show("Выберите члена семьи для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RefreshGrid()
        {
            dgFamilyMembers.ItemsSource = null;
            dgFamilyMembers.ItemsSource = _familyMembers;
        }
    }
}