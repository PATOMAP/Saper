using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Saper.MVVM.Model
{
    public class ButtonSaper
    {
        public int x;
        public int y;
        private bool _rightClick;

        private Button _buttonSp;

        private bool _infBomb;

        private int _countBomb;

        private bool _display;

        private bool _displayNew;

        private bool _poleStart;

        private bool _firstPoleAround;//status pola wokół pierwszego kliknięcia


        public bool PoleStart
        {
            get { return _poleStart; }
            set { _poleStart = value; }
        }

        public bool DisplayNew
        {
            get { return _displayNew; }
            set { _displayNew = value; }
        }

        public bool Display
        {
            get { return _display; }
            set { _display = value; }
        }

        public int CountBomb
        {
            get { return _countBomb; }
            set { _countBomb = value; }
        }

        public bool InfBomb
        {
            get { return _infBomb; }
            set { _infBomb = value; }
        }
        public Button ButtonSp
        {
            get { return _buttonSp; }
            set { _buttonSp = value; }
        }

        public bool RightClick
        {
            get { return _rightClick; }

            set { _rightClick = value; }//for tests
        }




        public bool FirstPoleAround { get => _firstPoleAround; set => _firstPoleAround = value; }

        public ButtonSaper(int x, int y)
        {
            _rightClick = false;
            Display = false;
            FirstPoleAround = false;
            ButtonSp = new Button();
            CountBomb = 0;
            PoleStart = false;
            TextBlock txt = new TextBlock() { Text = "?" };
            ButtonSp.Content = txt;

            this.x = x;
            this.y = y;

            ButtonSp.MouseRightButtonUp += ClickRightButton;
            ButtonSp.Click += ClickLeftButton;


        }

        private void ClickRightButton(object sender, RoutedEventArgs e)
        {

            Button btn = sender as Button;
            Grid grd = btn.Parent as Grid;
            if (Display == false)
            {
                if (grd.Tag == null)
                    _rightClick = !_rightClick;

                if (_rightClick && grd.Tag == null)
                {
                    Image img = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/Saper;component/Images/flag.png"))
                    };
                    ButtonSp.Content = img;

                }
                else
                {
                    TextBlock txt = new TextBlock() { Text = "?" };
                    ButtonSp.Content = txt;
                    if (grd.Tag != null)
                        _rightClick = false;
                }
            }



        }
        private void ClickLeftButton(object sender, RoutedEventArgs e)
        {
            if (Display != true)
            {
                DisplayNew = true;
                _rightClick = false;
            }

        }

    }

}