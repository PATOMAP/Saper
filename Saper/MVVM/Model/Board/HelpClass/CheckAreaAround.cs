using Saper.MVVM.Model.Board.Errors;
using Saper.MVVM.Model.Board.HelpClass.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saper.MVVM.Model.Board.HelpClass
{
    public class CheckAreaAround: ICheckAreaAround
    {
        List<ButtonSaper> _buttonsInGrid;
        public CheckAreaAround(List<ButtonSaper> buttonsInGrid)
        {
            _buttonsInGrid = buttonsInGrid;
        }
        public object Check(ButtonSaper buttonSaper)
        {
            if(buttonSaper==null)
            {
                return new ErrorButton();
            }
            else if (_buttonsInGrid == null)
            {
                return new ErrorArea();
            }
            for (int i = 0; i < 8; i++)
            {
                var (pozx, pozy) = CheckPoleAround.Check(buttonSaper.x, buttonSaper.y, i);
                if (_buttonsInGrid.Any(a => a.x == pozx && a.y == pozy))
                {
                    ButtonSaper bsOb = _buttonsInGrid.FirstOrDefault(a => a.x == pozx && a.y == pozy);
                    if (bsOb.InfBomb == false && bsOb.CountBomb == 0 && bsOb != null && bsOb.Display == false)
                    {
                        bsOb.Display = true;
                        BackgroudButton.ChangeButton(bsOb);
                        Check(bsOb);
                    }
                    if (buttonSaper.CountBomb == 0 && buttonSaper.InfBomb == false)
                    {
                        bsOb.Display = true;
                        BackgroudButton.ChangeButton(bsOb);
                    }

                }
            }
            return null;
        }
    }
}
