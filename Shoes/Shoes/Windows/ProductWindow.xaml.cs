using Shoes.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;

namespace Shoes.Windows
{
    /// <summary>
    /// Логика взаимодействия для ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        private List<Tovar> tovar = new List<Tovar>();
        private List<Tovar> filteredTovars = new List<Tovar>();
        private List<Supplier> suppliers = new List<Supplier>();
        public ProductWindow()
        {
            InitializeComponent();
            if (App.CurrentUser != null && App.CurrentUser.Role.RoleName == "Администратор")
            {
                name_tb.Text = $"{App.CurrentUser.Role.RoleName} {App.CurrentUser.SNL}";
                back_btn.Visibility = Visibility.Visible;
                login_btn.Visibility = Visibility.Collapsed;
                sortComboBox.Visibility = Visibility.Visible;
                filterComboBox.Visibility = Visibility.Visible;
                searchTextBox.Visibility = Visibility.Visible;
                filterTb.Visibility = Visibility.Visible;
                sortTb.Visibility = Visibility.Visible;
                searchTb.Visibility = Visibility.Visible;
                add_btn.Visibility = Visibility.Visible;
                menu.Visibility = Visibility.Visible;
            }
            else if (App.CurrentUser != null && App.CurrentUser.Role.RoleName == "Менеджер")
            {
                name_tb.Text = $"{App.CurrentUser.Role.RoleName} {App.CurrentUser.SNL}";
                back_btn.Visibility = Visibility.Visible;
                login_btn.Visibility = Visibility.Collapsed;
                sortComboBox.Visibility = Visibility.Visible;
                filterComboBox.Visibility = Visibility.Visible;
                searchTextBox.Visibility = Visibility.Visible;
                filterTb.Visibility = Visibility.Visible;
                sortTb.Visibility = Visibility.Visible;
                searchTb.Visibility = Visibility.Visible;
                add_btn.Visibility = Visibility.Collapsed;
                menu.Visibility = Visibility.Collapsed;
            }
            else if (App.CurrentUser != null && App.CurrentUser.Role.RoleName == "Авторизированный клиент")
            {
                name_tb.Text =  $"{App.CurrentUser.Role.RoleName} {App.CurrentUser.SNL}";
                back_btn.Visibility = Visibility.Visible;
                login_btn.Visibility = Visibility.Collapsed;
                sortComboBox.Visibility = Visibility.Collapsed;
                filterComboBox.Visibility = Visibility.Collapsed;
                searchTextBox.Visibility = Visibility.Collapsed;
                filterTb.Visibility = Visibility.Collapsed;
                sortTb.Visibility = Visibility.Collapsed;
                searchTb.Visibility = Visibility.Collapsed;
                add_btn.Visibility = Visibility.Collapsed;
                menu.Visibility = Visibility.Collapsed;
                tovar_btn.Visibility = Visibility.Collapsed;
            }
            else
            {
                back_btn.Visibility = Visibility.Collapsed;
                login_btn.Visibility = Visibility.Visible;
                sortComboBox.Visibility = Visibility.Collapsed;
                filterComboBox.Visibility = Visibility.Collapsed;
                searchTextBox.Visibility = Visibility.Collapsed;
                filterTb.Visibility = Visibility.Collapsed;
                sortTb.Visibility = Visibility.Collapsed;
                searchTb.Visibility = Visibility.Collapsed;
                add_btn.Visibility = Visibility.Collapsed;
                menu.Visibility = Visibility.Collapsed;
                tovar_btn.Visibility = Visibility.Collapsed;
            }
            
            LoadSupplier();
            LoadData();
        }

        private void LoadData()
        {
            using (var context = new DbShoesContext())
            {
                tovar = context.Tovars.Include(a => a.TovarCategory).Include(a => a.Manufacturer).Include(a => a.Supplier).ToList();
                if (tovar.Count == 0)
                {
                    MessageBox.Show("Не удалось загрузить товары или база данных пуста");
                }
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            tovar_lv.Items.Clear();
            if (tovar == null || !tovar.Any()) return;

            filteredTovars = tovar.ToList();

            if (!string.IsNullOrWhiteSpace(searchTextBox.Text))
            {
                var searchText = searchTextBox.Text.ToLower();
                filteredTovars = filteredTovars.Where(t =>
                    (t.TovarName?.ToLower() ?? "").Contains(searchText) ||
                    (t.TovarDescription?.ToLower() ?? "").Contains(searchText) ||
                    (t.Supplier?.SupplierName?.ToLower() ?? "").Contains(searchText) ||
                    (t.TovarCategory?.TovarCategoryName?.ToLower() ?? "").Contains(searchText) ||
                    (t.Manufacturer?.ManufacturerName?.ToLower() ?? "").Contains(searchText)).ToList();
            }


            if (filterComboBox.SelectedIndex > 0)
            {
                int supplierIndex = filterComboBox.SelectedIndex - 1;
                if (supplierIndex >= 0 && supplierIndex < suppliers.Count)
                {
                    var selectedSupplier = suppliers[supplierIndex];
                    filteredTovars = filteredTovars.Where(t => t.SupplierId == selectedSupplier.SupplierId).ToList();
                }
            }

            switch (sortComboBox.SelectedIndex)
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
                nullP.Text = "Товары не найдены";
            }
            else
            {
                nullP.Text = "";
            }

            foreach (var item in filteredTovars)
            {
                tovar_lv.Items.Add(new TovarControl(item));
            }

        }

        private void LoadSupplier()
        {
            using (var context = new DbShoesContext())
            {
                filterComboBox.Items.Clear();
                suppliers = context.Suppliers.ToList();
                filterComboBox.Items.Add("Все товары");

                foreach (var item in suppliers)
                {
                    filterComboBox.Items.Add(item.SupplierName);
                }

                filterComboBox.SelectedIndex = 0;
            }
        }

        private void redact_Click(object sender, RoutedEventArgs e)
        {
            if (tovar_lv.SelectedItem is TovarControl selectedCard)
            {
                if (selectedCard.DataContext is Tovar selectedTovar)
                {
                    AddTovarWindow addTovarWindow = new AddTovarWindow(selectedTovar);
                    addTovarWindow.Show();
                    this.Close();
                }
            }
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void sortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (tovar_lv.SelectedItem is TovarControl selectedCard)
            {
                if (selectedCard.DataContext is Tovar selectedTovar)
                {
                    MessageBoxResult result = MessageBox.Show("Вы точно хотите удалить товар?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            using (var db = new DbShoesContext())
                            {
                                Tovar product = db.Tovars.Find(selectedTovar.TovarArticle);

                                if (product == null)
                                {
                                    MessageBox.Show("Товар не найден в базе данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }

                                if (db.StructureOrders.Any(a => a.TovarArticle == product.TovarArticle))
                                {
                                    MessageBox.Show("Товар нельзя удалить, так как он есть в заказах", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }

                                db.Tovars.Remove(product);
                                db.SaveChanges();

                                MessageBox.Show( "Товар успешно удален", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);

                                LoadData();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private void back_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы точно хотите выйти?", "Подтверждение выхода", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
App.CurrentUser = null;
            back_btn.Visibility = Visibility.Collapsed;
            login_btn.Visibility = Visibility.Visible;
            sortComboBox.Visibility = Visibility.Collapsed;
            filterComboBox.Visibility = Visibility.Collapsed;
            searchTextBox.Visibility = Visibility.Collapsed;
            filterTb.Visibility = Visibility.Collapsed;
            sortTb.Visibility = Visibility.Collapsed;
            searchTb.Visibility = Visibility.Collapsed;
            add_btn.Visibility = Visibility.Collapsed;
            menu.Visibility = Visibility.Collapsed;
            tovar_btn.Visibility = Visibility.Collapsed;
            }
            
        }

        private void login_btn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            AddTovarWindow addTovarWindow = new AddTovarWindow(null);
            addTovarWindow.Show();
            this.Close();
        }

        private void tovar_btn_Click(object sender, RoutedEventArgs e)
        {
            OrderWindow productWindow = new OrderWindow();
            productWindow.Show();
            this.Close();
        }
    }
}
