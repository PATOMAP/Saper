using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Saper.Core;
using Saper.MVVM.Model;

namespace Saper.MVVM.ViewModel
{
     class MainViewModel:ObservableObject
    {



        private HomeViewModel HomeVm { get;  set; }
        
        
        public RelayCommand BackButton { get; private set; }

        private Visibility _visibleButton;
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        public Visibility VisibleButton
        {
            get => _visibleButton;
            set
            {
                _visibleButton = value;
                OnPropertyChanged(nameof(VisibleButton));
            }
        }
        public MainViewModel()
        {
            VisibleButton = Visibility.Collapsed;
            BackButton = new RelayCommand(o =>
            {
                
                string z =CurrentView.GetType().Name;
                if(z == "PlayerResultsViewModel" ||  z == "RulesViewModel" || z == "UserViewModel")
                {
                    CurrentView = HomeVm;
                    VisibleButton=Visibility.Collapsed;
                }
                else
                {
                    CurrentView =  new UserViewModel(this); 
                    VisibleButton = Visibility.Visible;
                }
                    
            });
            HomeVm = new HomeViewModel(this);

            CurrentView = HomeVm;
        }
    }
}
