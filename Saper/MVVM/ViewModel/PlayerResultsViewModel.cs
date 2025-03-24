using Saper.Core;
using Saper.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Saper.MVVM.Model.DataBase;
namespace Saper.MVVM.ViewModel
{
    class PlayerResultsViewModel : ObservableObject
    {
        public ObservableCollection<string> ItemsCombo { get; set; }

        private string _selectedItemCombo;
        private int _selectedItemTab;

        private DataTable _databaseTable;

        public RelayCommand ChangeTabComboControl { get; private set; }

        public DataView DataTable => _databaseTable.DefaultView;

        public int SelectedItemTab
        {
            get => _selectedItemTab;
            set
            {
                if (_selectedItemTab != value)
                {
                    _selectedItemTab = value;
                    OnPropertyChanged(nameof(SelectedItemTab));
                    ChangeTabComboControl.Execute(null); 
                }
            }
        }

        public string SelectedItemCombo
        {
            get => _selectedItemCombo;
            set
            {
                if (_selectedItemCombo != value)
                {
                    _selectedItemCombo = value;
                    OnPropertyChanged(nameof(SelectedItemCombo));
                    ChangeTabComboControl.Execute(null); 
                }
            }
        }

        public PlayerResultsViewModel()
        {
            ChangeTabComboControl = new RelayCommand(o =>
            {
                int ind = ItemsCombo.IndexOf(SelectedItemCombo) + 1;
               
                _databaseTable = DatabaseSaper.DisplayResults(ind, SelectedItemTab);
                if(SelectedItemTab == 0)
                    _databaseTable.DefaultView.Sort = "Time ASC";
                OnPropertyChanged(nameof(DataTable));
            });
            _databaseTable = new DataTable();
            ItemsCombo = new ObservableCollection<string> { "Easy", "Medium", "Hard" };
            SelectedItemCombo = ItemsCombo.FirstOrDefault();



            
            ChangeTabComboControl.Execute(null);
        }
    }
}
