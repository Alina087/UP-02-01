using Microsoft.EntityFrameworkCore;
using Shoes.Model;
using Shoes.Windows;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Shoes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void gost_btn_Click(object sender, RoutedEventArgs e)
        {
            ProductWindow productWindow = new ProductWindow();
            productWindow.Show();
            this.Close();
        }

        private void enter_btn_Click(object sender, RoutedEventArgs e)
        {
            string login = login_txt.Text;
            string pass = pass_txt.Text;
            using (var context = new DbShoesContext())
            {
                var user = context.Users.Include(a => a.Role).FirstOrDefault(a => a.UserLogin == login && a.UserPass == pass);
                if (user != null)
                {
                    App.CurrentUser = user;
                    ProductWindow productWindow = new ProductWindow();
                    productWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Логин или пароль неверный", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}