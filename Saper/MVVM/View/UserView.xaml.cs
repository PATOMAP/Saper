using Saper.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Saper.MVVM.View
{
    /// <summary>
    /// Interaction logic for UserView.xaml
    /// </summary>
    public partial class UserView : UserControl
    {

        public UserView()
        {
            InitializeComponent();
            

        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UserViewModel um = (UserViewModel)this.DataContext;
            PasswordBox passwordBox = (PasswordBox)sender;
            um.Password=passwordBox.Password;
        }
    }
}
