using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows;

namespace Saper.MVVM.Model
{
    public class BackgroudButton
    {



        public static void ChangeButton(ButtonSaper buttonSaper)
        {
            if (buttonSaper.Display == true)
            {
                if (buttonSaper.InfBomb)
                {
                    Image img = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/Saper;component/Images/bomba.png"))
                    };
                    buttonSaper.ButtonSp.Background = new SolidColorBrush(Colors.Red);
                    buttonSaper.ButtonSp.Content = img;

                }
                else if (buttonSaper.InfBomb == false && buttonSaper.CountBomb > 0)
                {
                    TextBlock txt = new TextBlock() { Text = buttonSaper.CountBomb.ToString() };
                    switch (buttonSaper.CountBomb)
                    {
                        case 1:
                            txt.Foreground = new SolidColorBrush(Colors.Red);
                            break;
                        case 2:
                            txt.Foreground = new SolidColorBrush(Colors.Green);
                            break;
                        case 3:
                            txt.Foreground = new SolidColorBrush(Colors.Blue);
                            break;
                        case 4:
                            txt.Foreground = new SolidColorBrush(Colors.Orange);
                            break;
                        case 5:
                            txt.Foreground = new SolidColorBrush(Colors.Purple);
                            break;
                        case 6:
                            txt.Foreground = new SolidColorBrush(Colors.Cyan);
                            break;
                        case 7:
                            txt.Foreground = new SolidColorBrush(Colors.Magenta);
                            break;
                        case 8:
                            txt.Foreground = new SolidColorBrush(Colors.Brown);
                            break;
                        default:
                            txt.Foreground = new SolidColorBrush(Colors.Gray);
                            break;
                    }
                    txt.FontWeight = FontWeights.Bold;
                    buttonSaper.ButtonSp.Content = txt;


                }
                else
                {
                    buttonSaper.ButtonSp.Content = null;
                }
            }

        }
    }
}
