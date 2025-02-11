using Saper.Core;
using Saper.MVVM.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Saper.MVVM.ViewModel
{
    class HomeViewModel:ObservableObject
    {
        private readonly MainViewModel _mainViewModel;
        public RelayCommand GameViewCommand { get; private set; }
        public RelayCommand UserViewCommand { get; private set; }
        public RelayCommand PlayerResultsViewCommand { get; private set; }
        public RelayCommand RulesViewCommand { get; private set; }


        public HomeViewModel(MainViewModel mainViewModel)
        {

            _mainViewModel = mainViewModel;

            UserViewCommand = new RelayCommand(o =>
            {
                _mainViewModel.CurrentView = new UserViewModel(_mainViewModel);
                _mainViewModel.VisibleButton = Visibility.Visible;
            });

            PlayerResultsViewCommand = new RelayCommand(o =>
            {
                _mainViewModel.CurrentView = new PlayerResultsViewModel();
                _mainViewModel.VisibleButton = Visibility.Visible;
            });

            RulesViewCommand = new RelayCommand(o =>
            {
                _mainViewModel.CurrentView = new RulesViewModel();
                _mainViewModel.VisibleButton = Visibility.Visible;
            });
        }

    }
}
