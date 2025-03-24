using Saper.Core;
using Saper.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Saper.MVVM.Model.DataBase;
namespace Saper.MVVM.ViewModel
{
    class GameViewModel : ObservableObject
    {
        private readonly MainViewModel _mainViewModel;
        private DispatcherTimer _timer;
        private PoleSaper _poleSaper;
        private UserInfo _user;
        private object _gameContent;
        private DateTime timeStart;
        private string _timeGame;
        private string _bestTimeGame;
        private UserRecord _userRecordAct;
        private UserRecord _userRecordBest;
        private string _flagCount;
        private string _bombCount;
        TimeSpan _elapsedTime;
        private int _statusGame;//0-no game 1-win 2-lose

        public RelayCommand NewGameCommand { get; private set; }
        public RelayCommand ReturnCommand { get; private set; }
        public string FlagCount
        {
            get { return _flagCount; }
            set { 
                if (_flagCount != value) 
                {
                    _flagCount = value;
                    OnPropertyChanged(nameof(FlagCount));
                } 
            }
        }

        public string BombCount
        {
            get { return _bombCount; }
            set
            {
                if (_bombCount != value)
                {
                    _bombCount = value;
                    OnPropertyChanged(nameof(BombCount));
                }
            }
        }

        public UserInfo User
        {
            get { return _user; }
            set { _user = value; }
        }

        public string UserName
        {
            get { return User.Name; }
            set { User.Name = value; }
        }
        public string TimeGame
        {
            get { return _timeGame; }
            set {
                if (_timeGame != value)
                {
                    _timeGame = value;
                    OnPropertyChanged(nameof(TimeGame)); 
                }
            }
        }
        public int CurrentStreak 
        {
            get { return _userRecordAct.Streak; }
            set
            {
                if (_userRecordAct.Streak != value)
                {
                    _userRecordAct.Streak = value;
                    OnPropertyChanged(nameof(CurrentStreak));
                }
            }
        }

        public string BestTime
        {
            get { return _bestTimeGame; }
            set
            {
                if (_bestTimeGame != value)
                {
                    _bestTimeGame = value;
                    OnPropertyChanged(nameof(BestTime));
                }
            }
        }
        public int BestStreak
        {
            get { return _userRecordBest.Streak; }
            set
            {
                if (_userRecordBest.Streak != value)
                {
                    _userRecordBest.Streak = value;
                    OnPropertyChanged(nameof(BestStreak));
                }
            }
        }
        public object GameContent
        {
            get => _gameContent;
            set
            {
                if (_gameContent != value)
                {
                    _gameContent = value;
                    OnPropertyChanged(nameof(GameContent));
                }
            }
        }



        public GameViewModel(MainViewModel mainViewModel,UserInfo userInfo)

        {
            _statusGame = 0;
             User = userInfo;
            UserName=User.Name;
            _mainViewModel = mainViewModel;

            _userRecordBest = DatabaseSaper.InfUser(userInfo);
            _userRecordAct = new UserRecord(userInfo);

            CurrentStreak=_userRecordAct.Streak;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(200);//200
            NewGame();
            NewGameCommand = new RelayCommand(o =>
            {
                NewGame();
            });
            ReturnCommand = new RelayCommand(o =>
            {
                _mainViewModel.CurrentView = new UserViewModel(_mainViewModel);
            });
        }
        private void Wait(object sender, EventArgs e)
        {
            string flagCount, bombCount; 
            bool odp = _poleSaper.WaitForClick(out flagCount, out bombCount);
            _statusGame = 0;
            FlagCount = flagCount;
            BombCount = bombCount;

            if (odp)
            {

                timeStart = DateTime.Now;
                _timer.Tick -= Wait;
                _timer.Tick += EventduringGame;
            }


        }
        private void EventduringGame(object sender, EventArgs e)
        {
            string flagCount;
            int odp = _poleSaper.EventInBoard(out flagCount);
            FlagCount = flagCount;
            _statusGame = odp;
            if (odp==1 || odp==2)
            {
                _timer.Tick -= EventduringGame;
                _timer.Stop();
                if (odp == 1)
                    WinGame();
                else
                    CurrentStreak = 0;
               

            }
            else
            {
                 _elapsedTime = DateTime.Now - timeStart;
                TimeGame = _elapsedTime.ToString(@"hh\:mm\:ss");

            }
        }
        private void NewGame()
        {
            
            BestStreak = _userRecordBest.Streak;
            BestTime = _userRecordBest.BestTime.ToString(@"hh\:mm\:ss");
            _timer.Tick -= EventduringGame;
            _timer.Tick += Wait;
            TimeGame = TimeSpan.Zero.ToString();
            _poleSaper = new PoleSaper(User.Level);
            GameContent = _poleSaper.GridSaper;
            _timer.Start();
        }
        private void WinGame()
        {
            bool changeFalg = false;
            CurrentStreak += 1;

            

            if(_userRecordAct.BestTime>_elapsedTime || _userRecordAct.BestTime==TimeSpan.Zero)
             _userRecordAct.BestTime = _elapsedTime;

            if(_userRecordAct.BestTime<_userRecordBest.BestTime || _userRecordBest.BestTime == TimeSpan.Zero)
            {
                _userRecordBest.BestTime = new TimeSpan(_userRecordAct.BestTime.Hours, _userRecordAct.BestTime.Minutes, _userRecordAct.BestTime.Seconds);
                changeFalg=true;
            }


            if (CurrentStreak > BestStreak)
            {
                BestStreak = CurrentStreak;
                changeFalg = true;

            }
            if (changeFalg)
                DatabaseSaper.UpdateResults(_userRecordBest);

        }

    }
}
