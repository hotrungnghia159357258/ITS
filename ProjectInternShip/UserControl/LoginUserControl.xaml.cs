using System;
using System.Media;
using System.Windows.Media;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ProjectInternShip.Models;
using Microsoft.Win32;
using ProjectInternShip.DBContexts;
using System.Windows.Controls.Primitives;

namespace ProjectInternShip.LoginUC
{
    /// <summary>
    /// Interaction logic for LoginFuntion.xaml
    /// </summary>
    public partial class LoginUserControl : UserControl
    {
        private data_sensor_DBcontexts information;

        public LoginUserControl()
        {
            InitializeComponent();
            information = new data_sensor_DBcontexts();
            initialzeAccounts();
        }
        #region display sounds
        public void displayMouseClick()
        {
            var player = new MediaPlayer();

            player.Open(new Uri("./Sounds/click_x.wav", UriKind.Relative));
            player.Play();
        }
        #endregion

        #region initialize some account
        public void initialzeAccounts()
        {
            var number = (from acc in information.accounts
                          select acc).Count();
            if (number <= 0)
            {
                information.accounts.Add(new account { IsAdmin = true, Username = "admin", Password = "123456" });
                information.accounts.Add(new account { IsAdmin = true, Username = "username1", Password = "abcdef" });
                information.SaveChanges();
            }
        }
        #endregion

        #region handle event Login_Click
        public void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var user = (from acc in information.accounts
                        where acc.Username == Username.Text
                        select acc).FirstOrDefault<account>();
            if (user != null && user.Password == Password.Text)
            {
                Username.Clear();
                Password.Clear();
                this.Visibility = Visibility.Collapsed;
                if (user.IsAdmin)
                {
                   ( (UserControl) ((Panel)this.Parent).FindName("adminControl") ).Visibility = Visibility.Visible;
                }
                else
                {
                    ( (StatusBar) ((Panel)this.Parent).FindName("StatusBar") ).Visibility = Visibility.Visible;
                    ( (Border)((Panel)this.Parent).FindName("Border1") ).Visibility = Visibility.Visible;
                    ((Border)((Panel)this.Parent).FindName("Border2")).Visibility = Visibility.Visible;
                    ((Border)((Panel)this.Parent).FindName("Border3")).Visibility = Visibility.Visible;
                }
            }
            else
            {
                MessageBox.Show("Password or Username incorrect !");
            }
        }
        #endregion
    }
}
