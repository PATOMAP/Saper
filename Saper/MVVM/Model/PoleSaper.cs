using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace Saper.MVVM.Model
{
    public class PoleSaper
    {
        private Grid _gridSaper;

        private List<ButtonSaper> _buttonsInGrid;
        private int _szRow;
        private int _szCol;
        private int _amB;
        private bool _startGame;
        private int _actFalg;
        

        
        public List<ButtonSaper> ButtonSapers {
            get {  return _buttonsInGrid; }
            set {  _buttonsInGrid = value; } 
        }

        public Grid GridSaper
        {
            get { return _gridSaper; }
            set { _gridSaper = value; }
        }

  

        public PoleSaper(int par)
        {

            ButtonSapers = new List<ButtonSaper>();
            GridSaper = new Grid();
            switch (par)
            {
                case 1:
                    InitializeGrid(320,400,8,10,40,10);
                    break;
                case 2:                   
                    InitializeGrid(378,486,14,18,27,40);
                    break;
                case 3:
                    InitializeGrid(380,456,20,24,19,99);
                    break;
            }
           

        }
        private void InitializeGrid(int height, int widht, int szRow, int szCol, int szB, int amB)
        {
            GridSaper.VerticalAlignment = VerticalAlignment.Center;
            GridSaper.HorizontalAlignment = HorizontalAlignment.Center;
            GridSaper.Height = height;
            GridSaper.Width = widht;
            GridSaper.MouseRightButtonDown += ButtonRightClick;
            //
            for (int i = 0; i < szRow; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(szB);
                GridSaper.RowDefinitions.Add(row);

                for (int j = 0; j < szCol; j++)
                {
                    ColumnDefinition column = new ColumnDefinition();
                    column.Width = new GridLength(szB);
                    GridSaper.ColumnDefinitions.Add(column);

                    ButtonSaper btnS = new ButtonSaper(i, j);

                    Grid.SetRow(btnS.ButtonSp, btnS.x);
                    Grid.SetColumn(btnS.ButtonSp, btnS.y);
                    ButtonSapers.Add(btnS);
                    GridSaper.Children.Add(ButtonSapers.Last().ButtonSp);

                }
            }
            _szCol = szCol;
            _szRow = szRow;
            _amB = amB;
            // RozBomb(amB, szCol, szRow);

        }

        public bool waitForClick(out string bombCount, out string flagCount)
        {
            bombCount = ":"+ _amB.ToString();
            flagCount = ":"+_amB.ToString();
            if (_buttonsInGrid.Any(a=>a.DisplayNew==true))
                {
                     ButtonSaper bs = _buttonsInGrid.FirstOrDefault(a => a.DisplayNew == true);
                      _startGame = true;
                     bs.FirstPoleAround = true;
                     bs.Display = true;
                     bs.DisplayNew = false;
                     BackgroudButton bb = new BackgroudButton(bs);
                  StartPole(bs);
                  RozBomb();
                  checkAreaAround(bs);

                    return true;
                }
                return false;

        }

        public int  duringGame(out string flagCount)
        {
             flagCount= ":" + (_amB - _actFalg).ToString(); 
            if (_buttonsInGrid.Any(a => a.DisplayNew == true))
            {
                ButtonSaper bs = _buttonsInGrid.FirstOrDefault(a => a.DisplayNew == true);
                bs.Display = true;
                bs.DisplayNew = false;
                BackgroudButton bb = new BackgroudButton(bs);
                if (bs.InfBomb==true)
                {
                    LoseGame();
                    MessageBox.Show("You lose!");
                    return 2;
                }
                checkAreaAround(bs);
                if(_buttonsInGrid
                    .Where(a=>a.InfBomb==false)
                    .All(b=>b.Display==true)
                    )
                {
                    MessageBox.Show("You win!");
                    return 1;
                }
                return 0;
            }
            _actFalg= _buttonsInGrid.Count(a=>a.RightClick==true);

            if(_amB - _actFalg>=0)
            flagCount=":" + (_amB- _actFalg).ToString();

            return 0;
        }

        private void LoseGame()
        {
            foreach (var b in _buttonsInGrid)
            {
                if(b.InfBomb == true)
                {
                    b.Display = true;
                    BackgroudButton bb = new BackgroudButton(b);
                }
                    
            }
        }
        private void WinGame()
        {
            
        }

        public void checkAreaAround(ButtonSaper bs)
        {
            for (int i = 0; i < 8; i++)
            {
                var (pozx, pozy) = checkButt(bs.x, bs.y, i);
                if (_buttonsInGrid.Any(a => a.x == pozx && a.y == pozy))
                {
                    ButtonSaper bsOb = _buttonsInGrid.FirstOrDefault(a => a.x == pozx && a.y == pozy);    
                    if (bsOb.InfBomb == false && bsOb.QuantBomb == 0 && bsOb!=null && bsOb.Display==false)
                    {
                        bsOb.Display = true;
                        BackgroudButton bb = new BackgroudButton(bsOb);
                        checkAreaAround(bsOb);
                    }
                    if (bs.QuantBomb == 0 && bs.InfBomb == false)
                    {
                        bsOb.Display = true;
                        BackgroudButton bb = new BackgroudButton(bsOb);
                    }

                }
            }
        }

        private void StartPole(ButtonSaper bs)
        {
            for(int i = 0;i<8;i++)
            {
                var (pozx, pozy) = checkButt(bs.x, bs.y, i);
                if (_buttonsInGrid.Any(a => a.x==pozx && a.y==pozy))
                {
                    ButtonSaper bsOb = _buttonsInGrid.FirstOrDefault(a => a.x == pozx && a.y == pozy);
                    bsOb.FirstPoleAround = true;

                }
            }
            
        }
        private void RozBomb()
        {
            var indeks = new List<(int, int)>();
            var random = new Random();

            for (int i = 0; i < _amB; i++)
            {
                int x = random.Next(0, _szRow);
                int y = random.Next(0, _szCol);

                bool bothValuesInRow = indeks.Any(row => row.Item1 == x && row.Item2 == y);
                bool btnDispaly= _buttonsInGrid.Any(a=>a.x == x && a.y == y && a.FirstPoleAround==true);
                if (bothValuesInRow || btnDispaly)
                    i--;

                else
                {
                    ButtonSaper btnS= _buttonsInGrid.FirstOrDefault(a => a.x == x && a.y == y);
                    btnS.InfBomb = true;
                    indeks.Add((x, y));
                }
                    

            }

            foreach (ButtonSaper btn in ButtonSapers)//przypisanie bomb
            {
                if (btn.InfBomb)
                {

                    for (int i = 0; i < 8; i++)
                    {
                        var (pozx, pozy) = checkButt(btn.x, btn.y, i);
                        int pozxx = pozx;
                        int pozyy = pozy;
                        ButtonSaper tempBtn = ButtonSapers.FirstOrDefault(a => a.x == (int)pozx && a.y == pozy);
                        if (tempBtn != null && tempBtn.InfBomb == false)
                        {
                            tempBtn.QuantBomb++;
                        }
                    }

                }
                else
                    btn.InfBomb = false;

            }   

        }

        private void ButtonRightClick(object sender, RoutedEventArgs e)//block more flags
        {
            if(_actFalg==_amB)
            {
                GridSaper.Tag = "f";
            }
            else
            {
                GridSaper.Tag = null;
            }
        }
        private (int,int) checkButt(int x,int y,int i)
        {
            int pozx=0;
            int pozy=0;
            switch(i)
            {
                case 0:
                    pozx = x-1;
                    pozy = y-1;
                    break;
                case 1:
                    pozx = x - 1;
                    pozy = y ;
                    break;

                case 2:
                    pozx = x - 1;
                    pozy = y + 1;
                    break;

                case 3:
                    pozx = x ;
                    pozy = y - 1;
                    break;

                case 4:
                    pozx = x ;
                    pozy = y + 1;
                    break;

                case 5:
                    pozx = x + 1;
                    pozy = y - 1;
                    break;

                case 6:
                    pozx = x + 1;
                    pozy = y;
                    break;
                case 7:
                    pozx = x + 1;
                    pozy = y + 1;
                    break;

            }


            return (pozx,pozy);
        }
    }
}
