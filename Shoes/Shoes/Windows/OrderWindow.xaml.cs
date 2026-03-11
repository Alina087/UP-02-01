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
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        public OrderWindow()
        {
            InitializeComponent();
            if (App.CurrentUser != null && App.CurrentUser.Role.RoleName == "Администратор")
            {
                name_tb.Text = App.CurrentUser.SNL;
                back_btn.Visibility = Visibility.Visible;
                login_btn.Visibility = Visibility.Collapsed;
                add_btn.Visibility = Visibility.Visible;
                menu.Visibility = Visibility.Visible;
            }
            else if (App.CurrentUser != null && App.CurrentUser.Role.RoleName == "Менеджер")
            {
                name_tb.Text = App.CurrentUser.SNL;
                back_btn.Visibility = Visibility.Visible;
                login_btn.Visibility = Visibility.Collapsed;
                add_btn.Visibility = Visibility.Collapsed;
                menu.Visibility = Visibility.Collapsed;

            }
            else if (App.CurrentUser != null && App.CurrentUser.Role.RoleName == "Авторизированный клиент")
            {
                name_tb.Text = App.CurrentUser.SNL;
                back_btn.Visibility = Visibility.Visible;
                login_btn.Visibility = Visibility.Collapsed;
                add_btn.Visibility = Visibility.Collapsed;
                menu.Visibility = Visibility.Collapsed;
            }
            else
            {
                back_btn.Visibility = Visibility.Collapsed;
                login_btn.Visibility = Visibility.Visible;
                add_btn.Visibility = Visibility.Collapsed;
                menu.Visibility = Visibility.Collapsed;
            }
            LoadOrder();
        }

        private void LoadOrder()
        {
            using (var context = new DbShoesContext())
            {
                order_lv.Items.Clear();
                var orders = context.Orders.Include(a => a.PickUpPoint).OrderByDescending(a => a.OrderDate).ToList();

                foreach (var item in orders)
                {
                    order_lv.Items.Add(new OrderControl(item));
                }

            }
        }

        private void redact_Click(object sender, RoutedEventArgs e)
        {
            if (order_lv.SelectedItem is OrderControl selectedCard)
            {
                if (selectedCard.DataContext is Order selectedTovar)
                {
                    AddOrderWindow addTovarWindow = new AddOrderWindow(selectedTovar);
                    addTovarWindow.Show();
                    this.Close();
                }
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (order_lv.SelectedItem is OrderControl selectedCard)
            {
                if (selectedCard.DataContext is Order selectedOrder)
                {
                    MessageBoxResult result = MessageBox.Show("Вы точно хотите удалить заказ?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            using (var db = new DbShoesContext())
                            {
                                Order order = db.Orders.Find(selectedOrder.OrderId);

                                if (order == null)
                                {
                                    MessageBox.Show("Заказ не найден в базе данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }

                                if (order.OrderStatus == "Завершен")
                                {
                                    MessageBox.Show("Завершенный заказ нельзя удалить", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }

                                var structureItems = db.StructureOrders.Where(so => so.OrderId == order.OrderId);
                                db.StructureOrders.RemoveRange(structureItems);

                                db.Orders.Remove(order);

                                db.SaveChanges();

                                MessageBox.Show("Заказ успешно удален", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);

                                LoadOrder();
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
            App.CurrentUser = null;
            ProductWindow productWindow = new ProductWindow();
            productWindow.Show();
            this.Close();
        }

        private void login_btn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            AddOrderWindow addTovarWindow = new AddOrderWindow(null);
            addTovarWindow.Show();
            this.Close();
        }

        private void tovar_btn_Click(object sender, RoutedEventArgs e)
        {
            ProductWindow productWindow = new ProductWindow();
            productWindow.Show();
            this.Close();
        }
    }
}