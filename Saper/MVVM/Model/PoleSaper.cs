using Saper.MVVM.Model.Board.HelpClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace Saper.MVVM.Model
{
    public class PoleSaper
    {
        private Grid _gridSaper;

        private List<ButtonSaper> _buttonsInGrid;
        private int _widthRow;
        private int _widthCol;
        private int _countBomb;
        private bool _startGame;
        private int _actFalg;
        private CheckAreaAround checkAreaAround;

        


        public Grid GridSaper
        {
            get { return _gridSaper; }
            set { _gridSaper = value; }
        }

        public List<ButtonSaper> Buttons//for test
        {
            get { return _buttonsInGrid; }
            set { _buttonsInGrid = value; }
        }

        public PoleSaper(int par)
        {
            
            _buttonsInGrid = new List<ButtonSaper>();
            GridSaper = new Grid();
            switch (par)
            {
                case 1:
                    InitializeGrid(320,400,8,10,40,10);//80 blocks
                    break;
                case 2:                   
                    InitializeGrid(378,486,14,18,27,40);//252 blocks
                    break;
                case 3:
                    InitializeGrid(380,456,20,24,19,99);//480 blocks
                    break;
            }
           

        }
        private void InitializeGrid(int heightBoard, int widthBoard, int widthRow, int szCol, int szB, int countBomb)
        {
            GridSaper.VerticalAlignment = VerticalAlignment.Center;
            GridSaper.HorizontalAlignment = HorizontalAlignment.Center;
            GridSaper.Height = heightBoard;
            GridSaper.Width = widthBoard;
           GridSaper.MouseRightButtonDown += ClickButtonRight;
            //
            for (int i = 0; i < widthRow; i++)
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
                    _buttonsInGrid.Add(btnS);
                    GridSaper.Children.Add(_buttonsInGrid.Last().ButtonSp);

                }
            }
            checkAreaAround=new CheckAreaAround(_buttonsInGrid);//add to check buttons
            
            _widthCol = szCol;
            _widthRow = widthRow;
            _countBomb = countBomb;
            

        }

        public bool WaitForClick(out string bombCount, out string flagCount)
        {
            bombCount = ":"+ _countBomb.ToString();
            flagCount = ":"+_countBomb.ToString();
            if (_buttonsInGrid.Any(a=>a.DisplayNew==true))
                {
                     ButtonSaper newButtonClick = _buttonsInGrid.FirstOrDefault(a => a.DisplayNew == true);
                      _startGame = true;
                     newButtonClick.FirstPoleAround = true;
                     newButtonClick.Display = true;
                     newButtonClick.DisplayNew = false;

                BackgroudButton.ChangeButton(newButtonClick);
                StartPole(newButtonClick);
                PlacementBomb.Placement(_buttonsInGrid, _widthRow, _widthCol, _countBomb); //RozBomb();
                checkAreaAround.Check(newButtonClick);
                   return true;
                }
                return false;

        }

        public int  EventInBoard(out string flagCount)
        {
             flagCount= ":" + (_countBomb - _actFalg).ToString(); 
            if (_buttonsInGrid.Any(a => a.DisplayNew == true))
            {
                
                ButtonSaper newButtonClick = _buttonsInGrid.FirstOrDefault(a => a.DisplayNew == true);
                newButtonClick.Display = true;
                newButtonClick.DisplayNew = false;
                BackgroudButton.ChangeButton(newButtonClick);
                if (newButtonClick.InfBomb==true)
                {
                    LoseGame();
                    MessageBox.Show("You lose!");
                    return 2;
                }
                checkAreaAround.Check(newButtonClick);
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

            if(_countBomb - _actFalg>=0)
            flagCount=":" + (_countBomb- _actFalg).ToString();

            return 0;
        }

        private void LoseGame()
        {
            foreach (var b in _buttonsInGrid)
            {
                if(b.InfBomb == true)
                {
                    b.Display = true;
                    BackgroudButton.ChangeButton(b);
                }
                    
            }
        }



        private void StartPole(ButtonSaper bs)
        {
            for(int i = 0;i<8;i++)
            {
                var (pozx, pozy) = CheckPoleAround.Check(bs.x, bs.y, i);
                if (_buttonsInGrid.Any(a => a.x==pozx && a.y==pozy))
                {
                    ButtonSaper bsOb = _buttonsInGrid.FirstOrDefault(a => a.x == pozx && a.y == pozy);
                    bsOb.FirstPoleAround = true;


                }
            }
            
        }

        private void ClickButtonRight(object sender, RoutedEventArgs e)//block more flags
        {

            if(_actFalg==_countBomb)

            {
                GridSaper.Tag = "f";
            }
            else
            {
                GridSaper.Tag = null;
            }
        }
        
    }
}
