using Saper.MVVM.Model.Board.HelpClass.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saper.MVVM.Model.Board.HelpClass
{
    public class CheckPoleAround
    {

        public static (int, int) Check(int x, int y, int i)
        {
            int pozx = 0;
            int pozy = 0;
            switch (i)
            {
                case 0:
                    pozx = x - 1;
                    pozy = y - 1;
                    break;
                case 1:
                    pozx = x - 1;
                    pozy = y;
                    break;

                case 2:
                    pozx = x - 1;
                    pozy = y + 1;
                    break;

                case 3:
                    pozx = x;
                    pozy = y - 1;
                    break;

                case 4:
                    pozx = x;
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


            return (pozx, pozy);
        }
    }
}
