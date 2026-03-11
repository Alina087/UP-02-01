using Shoes.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Shoes.Windows
{
    public partial class AddOrderWindow : Window
    {
        private Order order;
        private Order existingOrder;
        private List<StructureOrder> orderItems = new List<StructureOrder>();
        private static Random random = new Random();

        public AddOrderWindow(Order data = null)
        {
            InitializeComponent();
            existingOrder = data;

            List<string> statuses = new List<string> { "Новый", "Завершен" };
            cbStatus.ItemsSource = statuses;

            using (var db = new DbShoesContext())
            {
                cmbTovar.ItemsSource = db.Tovars.ToList();
                cmbTovar.DisplayMemberPath = "TovarName";

                cmbPickUpPoint.ItemsSource = db.PickUpPoints.ToList();
                cmbPickUpPoint.DisplayMemberPath = "FullAddress";
            }

            dateOrder.DisplayDateStart = DateTime.Today;
            dateOrder.DisplayDateEnd = DateTime.Today.AddYears(1);
            dateOrder.SelectedDate = DateTime.Today;

            if (existingOrder != null)
            {
                FillFields(existingOrder);
                
            }
            else
            {
                GenerateUniqueOrderCode();
                cbStatus.SelectedItem = "Новый";
                cbStatus.IsEnabled = false;
            }
        }

        private void GenerateUniqueOrderCode()
        {
            using (var db = new DbShoesContext())
            {
                string newCode;
                do
                {
                    newCode = random.Next(100, 1000).ToString();
                }
                while (db.Orders.Any(o => o.OrderCode == newCode));

                txtOrderCode.Text = newCode;
            }
        }

        private void btnAddTovar_Click(object sender, RoutedEventArgs e)
        {
            var selectedTovar = cmbTovar.SelectedItem as Tovar;
            if (selectedTovar == null)
            {
                MessageBox.Show("Выберите товар", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int currentQuantity = orderItems.Where(oi => oi.TovarArticle == selectedTovar.TovarArticle).Sum(oi => oi.StructureOrderTovarCount ?? 0);
            if (selectedTovar.TovarCount < currentQuantity + quantity)
            {
                MessageBox.Show($"Недостаточно товара. Доступно: {selectedTovar.TovarCount}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var existing = orderItems.FirstOrDefault(oi => oi.TovarArticle == selectedTovar.TovarArticle);
            if (existing != null)
            {
                existing.StructureOrderTovarCount += quantity;
            }
            else
            {
                orderItems.Add(new StructureOrder
                {
                    TovarArticle = selectedTovar.TovarArticle,
                    TovarArticleNavigation = selectedTovar,
                    StructureOrderTovarCount = quantity
                });
            }

            lstOrderTovars.ItemsSource = null;
            lstOrderTovars.ItemsSource = orderItems;
            txtQuantity.Text = "1";
            cmbTovar.SelectedIndex = -1;
        }

        private void btnRemoveTovar_Click(object sender, RoutedEventArgs e)
        {
            if (lstOrderTovars.SelectedItem is StructureOrder selected)
            {
                orderItems.Remove(selected);
                lstOrderTovars.ItemsSource = null;
                lstOrderTovars.ItemsSource = orderItems;
            }
        }

        private void SaveGood_Click(object sender, RoutedEventArgs e)
        {
            if (orderItems.Count == 0)
            {
                MessageBox.Show("Добавьте товары в заказ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (cbStatus.SelectedItem == null)
            {
                MessageBox.Show("Выберите статус", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (cmbPickUpPoint.SelectedItem == null)
            {
                MessageBox.Show("Выберите пункт выдачи", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!dateOrder.SelectedDate.HasValue || !dateDelivery.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите даты", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime orderDate = dateOrder.SelectedDate.Value;
            DateTime deliveryDate = dateDelivery.SelectedDate.Value;

            // Проверки дат
            if (existingOrder == null && orderDate.Date != DateTime.Today)
            {
                MessageBox.Show("Дата заказа должна быть сегодня", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (deliveryDate.Date < orderDate.Date)
            {
                MessageBox.Show("Дата доставки не может быть раньше даты заказа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (deliveryDate.Date > orderDate.Date.AddYears(1))
            {
                MessageBox.Show("Дата доставки не может быть позже года", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка статуса для завершенных
            if (existingOrder != null && existingOrder.OrderStatus == "Завершен" && cbStatus.Text != "Завершен")
            {
                MessageBox.Show("Нельзя изменить статус завершенного заказа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var db = new DbShoesContext())
                {
                    if (existingOrder != null)
                    {
                        // Находим заказ по OrderId (int)
                        order = db.Orders.Find(existingOrder.OrderId);
                        if (order == null)
                        {
                            MessageBox.Show("Заказ не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        // Удаляем старые позиции
                        var oldItems = db.StructureOrders.Where(so => so.OrderId == order.OrderId);
                        db.StructureOrders.RemoveRange(oldItems);
                    }
                    else
                    {
                        // Проверяем, что сгенерированный код еще не занят
                        if (db.Orders.Any(o => o.OrderCode == txtOrderCode.Text))
                        {
                            // Если код занят, генерируем новый
                            GenerateUniqueOrderCode();
                            MessageBox.Show("Код заказа был изменен, повторите сохранение", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }

                        order = new Order();
                        db.Orders.Add(order);
                    }

                    // Заполняем заказ
                    order.OrderCode = txtOrderCode.Text;
                    order.OrderStatus = cbStatus.Text;
                    order.OrderDate = DateOnly.FromDateTime(orderDate);
                    order.OrderDateDelivery = DateOnly.FromDateTime(deliveryDate);
                    order.PickUpPointId = ((PickUpPoint)cmbPickUpPoint.SelectedItem).PickUpPointId;
                    order.UserId = App.CurrentUser?.UserId;

                    db.SaveChanges(); // Чтобы получить OrderId

                    // Добавляем позиции и обновляем остатки
                    foreach (var item in orderItems)
                    {
                        var tovar = db.Tovars.Find(item.TovarArticle);
                        if (tovar != null)
                        {
                            tovar.TovarCount -= item.StructureOrderTovarCount ?? 0;

                            db.StructureOrders.Add(new StructureOrder
                            {
                                OrderId = order.OrderId,
                                TovarArticle = item.TovarArticle,
                                StructureOrderTovarCount = item.StructureOrderTovarCount
                            });
                        }
                    }

                    db.SaveChanges();

                    string message = existingOrder != null ? "Заказ обновлен" : "Заказ добавлен";
                    MessageBox.Show(message, "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);

                    OrderWindow mainWindow = new OrderWindow();
                    mainWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FillFields(Order order)
        {
            txtOrderCode.Text = order.OrderCode;
            cbStatus.Text = order.OrderStatus;

            if (order.OrderStatus == "Завершен")
            {
                cbStatus.IsEnabled = false;
                cmbPickUpPoint.IsEnabled = false;
                cmbTovar.IsEnabled = false;
                txtQuantity.IsEnabled = false;
                cmbPickUpPoint.IsEnabled = false;
                dateOrder.IsEnabled = false;
                dateDelivery.IsEnabled = false;
            }
                

            if (order.OrderDate.HasValue)
                dateOrder.SelectedDate = order.OrderDate.Value.ToDateTime(TimeOnly.MinValue);

            if (order.OrderDateDelivery.HasValue)
                dateDelivery.SelectedDate = order.OrderDateDelivery.Value.ToDateTime(TimeOnly.MinValue);

            foreach (PickUpPoint p in cmbPickUpPoint.Items)
            {
                if (p.PickUpPointId == order.PickUpPointId)
                {
                    cmbPickUpPoint.SelectedItem = p;
                    break;
                }
            }

            using (var db = new DbShoesContext())
            {
                var items = db.StructureOrders.Where(so => so.OrderId == order.OrderId).ToList();
                foreach (var item in items)
                {
                    item.TovarArticleNavigation = db.Tovars.Find(item.TovarArticle);
                    orderItems.Add(item);
                }
                lstOrderTovars.ItemsSource = orderItems;
            }
        }

        private void backMain_btn_Click(object sender, RoutedEventArgs e)
        {
            OrderWindow orderWindow = new OrderWindow();
            orderWindow.Show();
            this.Close();
        }

        private void back_btn_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            ProductWindow productWindow = new ProductWindow();
            productWindow.Show();
            this.Close();
        }
    }
}