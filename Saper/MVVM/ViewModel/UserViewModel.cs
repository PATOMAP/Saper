using Saper.Core;
using Saper.MVVM.Model;
using Saper.MVVM.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Drawing;
using Saper.MVVM.Model.DataBase;



namespace Saper.MVVM.ViewModel
{
    class UserViewModel : ObservableObject
    {
        private readonly MainViewModel _mainViewModel;
        private double _sliderValue;
        private SolidColorBrush _colorText;
        private UserInfo _user;
        private string _userName;
        private string _displayedText;
        private string _password;
        private bool _newUser;
        

        public string UserName
        {
            get { return _userName; }
            set { 
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }

        }



        public UserInfo User
        {
            get { return _user; }
            set { _user = value; }
        }
        public bool NewUser
        {
            get { return _newUser; }
            set { 
                if(_newUser!=value)
                {
                    _newUser = value;
                    OnPropertyChanged(nameof(NewUser));
                }

            }
        }
        public int SliderValue
        {
            get => User.Level;
            set
            {
                if (User.Level != value)
                {
                    
                    User.Level = value;
                    OnPropertyChanged(nameof(User.Level));

                    ChangeTextCommand.Execute(User.Level);
                }
            }
        }
        public string NameValue
        {
            get => User.Name;
            set
            {
                if (User.Name != value)
                {
                    User.Name = value;
                    OnPropertyChanged(nameof(NameValue));

                }
            }
        }

        public string DisplayedText
        {
            get => _displayedText;
            set
            {
                if (_displayedText != value)
                {
                    _displayedText = value;
                    OnPropertyChanged(nameof(DisplayedText));
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));

                }
            }
        }

        
        public SolidColorBrush ColorText
        {
            get => _colorText;
            set
            {
                if (_colorText != value)
                {
                    _colorText = value;
                    OnPropertyChanged(nameof(ColorText));

                    
                }
            }
        }

        public RelayCommand GameViewCommand { get; private set; }
        public RelayCommand ChangeTextCommand { get; }
        public UserViewModel(MainViewModel mainViewModel)
        {

            _user = new UserInfo();
            _mainViewModel= mainViewModel;

            GameViewCommand = new RelayCommand(o =>
            {

                


                if (String.IsNullOrEmpty(User.Name) || String.IsNullOrEmpty(Password))
                {
                    MessageBox.Show("Empty Name or Password!");
                    return;
                }
               bool odp= DatabaseSaper.CheckName(User.Name, _password, NewUser);
                if (odp)
                {
                    _mainViewModel.CurrentView = new GameViewModel(_mainViewModel, User);
                }
                else
                    return;
                    
              


            });

            ChangeTextCommand = new RelayCommand(o =>
            {
                object z = o;
                if (o is int value)
                {

                    int intValue = (int)value; 
                    switch (intValue)
                    {
                        case 1:
                            DisplayedText = "Easy";
                            ColorText= new SolidColorBrush(Colors.Green);
                            break;
                        case 2:
                            DisplayedText = "Medium";
                            ColorText = new SolidColorBrush(Colors.Orange);
                            break;
                        case 3:
                            DisplayedText = "Hard";
                            ColorText = new SolidColorBrush(Colors.Red);
                            break;
                        default:
                            DisplayedText = "Unknown Option";
                            break;
                    }
                }
            });
            SliderValue = 1;

        }





    }
}
