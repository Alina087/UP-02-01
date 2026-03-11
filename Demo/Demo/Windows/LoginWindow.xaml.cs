using Demo.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;

namespace Demo.Windows
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (var db = new DbShoes1Context())
            {
                var user = db.Users.Include(a => a.Role).FirstOrDefault(a => a.UserLogin == login.Text && a.UserPass == pass.Text);
                if (user != null)
                {
                    App.CurrentUser = user;
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                    
                }
                else
                {
                    MessageBox.Show("Логин или пароль неверный", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
               

            }
        }
    }
}
