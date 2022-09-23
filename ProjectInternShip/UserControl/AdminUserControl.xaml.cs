using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ProjectInternShip.DBContexts;
//using ProjectInternShip.InfoDB;
using ProjectInternShip.LoginUC;
using ProjectInternShip.Models;

namespace ProjectInternShip.AdminUC
{
    public partial class AdminUserControl : UserControl
    {
        private data_sensor_DBcontexts information;

        public AdminUserControl()
        {
            InitializeComponent();
            information = new data_sensor_DBcontexts();
            displayListUser();
        }

        public void displayListUser()
        {
            var lst = (from acc in information.accounts
                       where acc.IsAdmin == false
                       select acc).ToList();
            ListUser.ItemsSource = lst;
        }

        public void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            if (NewPassword.Text == "" || NewPassword.Text == "")
            {
                MessageBox.Show("You must complete the form New User Acccount before click button Add User");
            }
            else
            {
                information.accounts.Add(new account { IsAdmin = false, Username = NewUsername.Text, Password = NewPassword.Text });
                information.SaveChanges();
                NewUsername.Clear();
                NewPassword.Clear();
                displayListUser();
            }        
        }

        public void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {   
            if (ListUser.SelectedItem == null)
            {
                MessageBox.Show("You must select an user account!");
            }
            else
            {
                account acc = (account)ListUser.SelectedItem;
                information.accounts.Remove(acc);
                information.SaveChanges();
                displayListUser();
            }          
        }

        public void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            ((Panel)this.Parent).Children[0].Visibility = Visibility.Visible;
        }
    }
}
