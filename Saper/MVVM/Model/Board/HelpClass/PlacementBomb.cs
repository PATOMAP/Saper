using Saper.MVVM.Model.Board.HelpClass.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Saper.MVVM.Model.Board.HelpClass
{
    public class PlacementBomb
    {
        public static void Placement(List<ButtonSaper> ButtonSapers,int widthRow,int widthCol,int countBomb)
        {
            var indeks = new List<(int, int)>();
            var random = new Random();

            for (int i = 0; i < countBomb; i++)
            {
                int x = random.Next(0, widthRow);
                int y = random.Next(0, widthCol);

                bool bothValuesInRow = indeks.Any(row => row.Item1 == x && row.Item2 == y);
                bool btnDispaly = ButtonSapers.Any(a => a.x == x && a.y == y && a.FirstPoleAround == true);
                if (bothValuesInRow || btnDispaly)
                    i--;

                else
                {
                    ButtonSaper btnS = ButtonSapers.FirstOrDefault(a => a.x == x && a.y == y);
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
                        var (pozx, pozy) = CheckPoleAround.Check(btn.x, btn.y, i);
                        int pozxx = pozx;
                        int pozyy = pozy;
                        ButtonSaper tempBtn = ButtonSapers.FirstOrDefault(a => a.x == pozxx && a.y == pozyy);
                        if (tempBtn != null && tempBtn.InfBomb == false)
                        {
                            tempBtn.CountBomb++;
                            if (tempBtn.CountBomb > 9)
                                MessageBox.Show("s");
                        }
                    }

                }
                else
                    btn.InfBomb = false;

            }

        }
    }
}
