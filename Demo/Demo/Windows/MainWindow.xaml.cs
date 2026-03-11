using Demo.Model;
using Microsoft.EntityFrameworkCore;
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
using Demo.Windows;

namespace Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
   
    public partial class MainWindow : Window
    {
        private List<Tovar> tovar = new List<Tovar>();
        private List<Supplier> suppliers = new List<Supplier>();
        private List<Tovar> filteredTovars = new List<Tovar>();
        public MainWindow()
        {
            InitializeComponent();
            LoadData();
            LoadSupplier();
            if (App.CurrentUser != null && App.CurrentUser.Role.RoleName == "Авторизированный клиент")
            {
                name_tb.Text = $"{App.CurrentUser.Role.RoleName}: {App.CurrentUser.UserLastname} {App.CurrentUser.UserName} {App.CurrentUser.UserSurname}";
                login.Visibility = Visibility.Collapsed;
                back.Visibility = Visibility.Visible;
            }
            else if (App.CurrentUser != null && App.CurrentUser.Role.RoleName == "Менеджер")
            {
                name_tb.Text = $"{App.CurrentUser.Role.RoleName}: {App.CurrentUser.UserLastname} {App.CurrentUser.UserName} {App.CurrentUser.UserSurname}";
                login.Visibility = Visibility.Collapsed;
                back.Visibility = Visibility.Visible;
            }
            else if (App.CurrentUser != null && App.CurrentUser.Role.RoleName == "Администратор")
            {
                name_tb.Text = $"{App.CurrentUser.Role.RoleName}: {App.CurrentUser.UserLastname} {App.CurrentUser.UserName} {App.CurrentUser.UserSurname}";
                login.Visibility = Visibility.Collapsed;
                back.Visibility = Visibility.Visible;
            }
            else
            {
                login.Visibility = Visibility.Visible;
                back.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadData()
        {
            using (var db = new DbShoes1Context())
            {
                tovar = db.Tovars.Include(a => a.Manufacturer).Include(a => a.Supplier).Include(a => a.TovarCategory).ToList();
                foreach (var item in tovar)
                {
                    tovar_lv.Items.Add(new TovarControl(item));
                }

                Filtered();
            }
        }

        private void Filtered()
        {
            tovar_lv.Items.Clear();
            if (tovar == null || !tovar.Any()) return;

            filteredTovars = tovar.ToList();

            if (!string.IsNullOrWhiteSpace(search.Text))
            {
                var searchText = search.Text.ToLower();
                filteredTovars = filteredTovars.Where(t =>
                    (t.TovarName?.ToLower() ?? "").Contains(searchText) ||
                    (t.TovarDescription?.ToLower() ?? "").Contains(searchText) ||
                    (t.Supplier?.SupplierName?.ToLower() ?? "").Contains(searchText) ||
                    (t.TovarCategory?.TovarCategoryName?.ToLower() ?? "").Contains(searchText) ||
                    (t.Manufacturer?.ManufacturerName?.ToLower() ?? "").Contains(searchText)).ToList();
            }


            if (filter.SelectedIndex > 0)
            {
                int supplierIndex = filter.SelectedIndex - 1;
                if (supplierIndex >= 0 && supplierIndex < suppliers.Count)
                {
                    var selectedSupplier = suppliers[supplierIndex];
                    filteredTovars = filteredTovars.Where(t => t.SupplierId == selectedSupplier.SupplierId).ToList();
                }
            }

            switch (sort.SelectedIndex)
            {
                case 0:
                    filteredTovars = filteredTovars.OrderBy(t => t.TovarCount).ToList();
                    break;
                case 1:
                    filteredTovars = filteredTovars.OrderByDescending(t => t.TovarCount).ToList();
                    break;
            }

            if (filteredTovars.Count == 0)
            {
                notTovar.Text = "Товары не найдены";
            }
            else
            {
                notTovar.Text = "";
            }

            foreach (var item in filteredTovars)
            {
                tovar_lv.Items.Add(new TovarControl(item));
            }
        }

        private void LoadSupplier()
        {
            using (var context = new DbShoes1Context())
            {
                filter.Items.Clear();
                suppliers = context.Suppliers.ToList();
                filter.Items.Add("Все товары");

                foreach (var item in suppliers)
                {
                    filter.Items.Add(item.SupplierName);
                }

                filter.SelectedIndex = 0;
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            login.Visibility = Visibility.Visible;
            back.Visibility = Visibility.Collapsed;

        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void search_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filtered();
        }

        private void sort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filtered();
        }

        private void filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filtered();
        }
    }
}