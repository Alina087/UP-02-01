using Microsoft.Win32;
using Shoes.Model;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Shoes.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddTovarWindow.xaml
    /// </summary>
    public partial class AddTovarWindow : Window
    {
        private Tovar product;
        private Tovar existingProduct;
        private string selectedImageName = "";
        private string oldImageName = "";

        private string resourcesPath = @"D:\проекты сишарп\Shoes\Shoes\Resources\";

        private static Random random = new Random();
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public AddTovarWindow(Tovar data = null)
        {
            InitializeComponent();
            existingProduct = data;
            if (existingProduct == null)
            {
                GenerateUniqueArticle();
            }

            cbCategories.ItemsSource = GetCategories();
            delivery.ItemsSource = GetSup();
            manufacturer.ItemsSource = GetMunuf();
            

            if (existingProduct != null)
            {
                FillFields(existingProduct);
            }
        }

        private void GenerateUniqueArticle()
        {
            using (var db = new DbShoesContext())
            {
                string newArticle;
                do
                {
                    newArticle = new string(Enumerable.Repeat(chars, 6)
                        .Select(s => s[random.Next(s.Length)]).ToArray());
                }
                while (db.Tovars.Any(t => t.TovarArticle == newArticle));

                article.Text = newArticle;
            }
        }

        private void add_image_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*",
                Title = "Выберите изображение товара"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedPath = openFileDialog.FileName;

                try
                {
                    // Проверяем размер изображения
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(selectedPath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    if (bitmap.PixelWidth != 300 || bitmap.PixelHeight != 200)
                    {
                        MessageBox.Show($"Размер изображения должен быть 300x200 пикселей.\nТекущий размер: {bitmap.PixelWidth}x{bitmap.PixelHeight} пикселей",
                            "Неверный размер", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    string savedFileName = SaveImageToResources(selectedPath);

                    if (!string.IsNullOrEmpty(savedFileName))
                    {
                        selectedImageName = savedFileName;

                        image.Source = new BitmapImage(new Uri(Path.Combine(resourcesPath, savedFileName)));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private string SaveImageToResources(string imagePath)
        {
            try
            {
                // Генерируем имя файла
                string extension = Path.GetExtension(imagePath).ToLower();
                string baseFileName = "";

                if (!string.IsNullOrEmpty(article.Text))
                {
                    baseFileName = CleanFileName(article.Text);
                }
                else if (existingProduct != null && !string.IsNullOrEmpty(existingProduct.TovarArticle))
                {
                    baseFileName = CleanFileName(existingProduct.TovarArticle);
                }
                else
                {
                    baseFileName = "product_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                }

                string newFileName = $"{baseFileName}{extension}";
                string destinationPath = Path.Combine(resourcesPath, newFileName);

                int counter = 1;
                while (File.Exists(destinationPath))
                {
                    newFileName = $"{baseFileName}_{counter}{extension}";
                    destinationPath = Path.Combine(resourcesPath, newFileName);
                    counter++;
                }

                File.Copy(imagePath, destinationPath, true);

                if (!string.IsNullOrEmpty(oldImageName) && oldImageName != "picture.png" && oldImageName != newFileName)
                {
                    string oldImagePath = Path.Combine(resourcesPath, oldImageName);
                    if (File.Exists(oldImagePath) && oldImageName != "picture.png")
                    {
                        try
                        {
                            File.Delete(oldImagePath);
                        }
                        catch { }
                    }
                }

                return newFileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private string CleanFileName(string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }

        private void LoadImageFromResources(string imageName)
        {
            try
            {
                string imagePath = Path.Combine(resourcesPath,
                    (string.IsNullOrEmpty(imageName) || imageName == "-" ? "picture.png" : imageName));

                if (File.Exists(imagePath))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    image.Source = bitmap;
                    oldImageName = imageName ?? "picture.png";
                }
                else
                {
                    ShowDefaultImage();
                }
            }
            catch
            {
                ShowDefaultImage();
            }
        }

        private void ShowDefaultImage()
        {
            try
            {
                string defaultImagePath = Path.Combine(resourcesPath, "picture.png");
                if (File.Exists(defaultImagePath))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(defaultImagePath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    image.Source = bitmap;
                }
                else
                {
                    image.Source = null;
                }
            }
            catch
            {
                image.Source = null;
            }
            oldImageName = "picture.png";
        }

        public static List<string> GetCategories()
        {
            using (var db = new DbShoesContext())
            {
                try
                {
                    return db.TovarCategories.Select(m => m.TovarCategoryName).ToList();
                }
                catch (Exception ex) { return null; }
            }
        }

        public static List<string> GetSup()
        {
            using (var db = new DbShoesContext())
            {
                try
                {
                    return db.Suppliers.Select(m => m.SupplierName).ToList();
                }
                catch (Exception ex) { return null; }
            }
        }

        public static List<string> GetMunuf()
        {
            using (var db = new DbShoesContext())
            {
                try
                {
                    return db.Manufacturers.Select(m => m.ManufacturerName).ToList();
                }
                catch (Exception ex) { return null; }
            }
        }

        private void SaveGood_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(name.Text))
            {
                MessageBox.Show("Заполните название товара", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                name.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(description.Text))
            {
                MessageBox.Show("Заполните описание товара", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                description.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(unit.Text))
            {
                MessageBox.Show("Заполните единицу измерения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                unit.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(cost.Text))
            {
                MessageBox.Show("Заполните стоимость товара", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                cost.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(discount.Text))
            {
                MessageBox.Show("Заполните действующую скидку", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                discount.Focus();
                return;
            }

            if (manufacturer.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите производителя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                manufacturer.Focus();
                return;
            }

            if (delivery.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите поставщика", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                delivery.Focus();
                return;
            }

            if (cbCategories.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите категорию товара", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                cbCategories.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(quantity.Text))
            {
                MessageBox.Show("Укажите количество товара на складе", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                quantity.Focus();
                return;
            }

            if (name.Text.Length > 45)
            {
                MessageBox.Show("Название не может быть длиннее 45 символов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                name.Focus();
                return;
            }

            if (!decimal.TryParse(cost.Text, out decimal costValue))
            {
                MessageBox.Show("Стоимость должна быть числовой", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                cost.Focus();
                return;
            }

            if (costValue <= 0)
            {
                MessageBox.Show("Стоимость не может быть отрицательной или равной 0", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                cost.Focus();
                return;
            }

            if (!int.TryParse(discount.Text, out int discountValue))
            {
                MessageBox.Show("Скидка должна содержать целое число", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                discount.Focus();
                return;
            }

            if (discountValue < 0)
            {
                MessageBox.Show("Скидка не может быть отрицательной", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                discount.Focus();
                return;
            }

            if (discountValue > 99)
            {
                MessageBox.Show("Скидка не может быть больше 99%", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                discount.Focus();
                return;
            }

            if (!int.TryParse(quantity.Text, out int quantityValue))
            {
                MessageBox.Show("Количество должно содержать целое число", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                quantity.Focus();
                return;
            }

            if (quantityValue <= 0)
            {
                MessageBox.Show("Количество не может быть меньше или равно 0", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                quantity.Focus();
                return;
            }

            try
            {
                using (var db = new DbShoesContext())
                {
                    if (existingProduct != null)
                    {
                        product = db.Tovars.Find(existingProduct.TovarArticle);
                        if (product == null)
                        {
                            MessageBox.Show("Товар не найден в базе данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    else
                    {

                        product = new Tovar();
                        product.TovarArticle = article.Text;
                        product.TovarName = name.Text;
                        product.TovarUnit = unit.Text;
                        product.TovarDescription = description.Text;
                        product.TovarCost = costValue;
                        product.TovarDiscount = discountValue;
                        product.TovarCount = quantityValue;
                        product.ManufacturerId = manufacturer.SelectedIndex + 1;
                        product.SupplierId = delivery.SelectedIndex + 1;
                        product.TovarCategoryId = cbCategories.SelectedIndex + 1;

                        if (!string.IsNullOrEmpty(selectedImageName))
                        {
                            product.TovarImage = selectedImageName;
                        }
                        else if (existingProduct == null || string.IsNullOrEmpty(product.TovarImage))
                        {
                            product.TovarImage = "picture.png";
                        }
                        db.Tovars.Add(product);
                    }

                    product.TovarArticle = article.Text;
                    product.TovarName = name.Text;
                    product.TovarUnit = unit.Text;
                    product.TovarDescription = description.Text;
                    product.TovarCost = costValue;
                    product.TovarDiscount = discountValue;
                    product.TovarCount = quantityValue;
                    product.ManufacturerId = manufacturer.SelectedIndex + 1;
                    product.SupplierId = delivery.SelectedIndex + 1;
                    product.TovarCategoryId = cbCategories.SelectedIndex + 1;

                    if (!string.IsNullOrEmpty(selectedImageName))
                    {
                        product.TovarImage = selectedImageName;
                    }
                    else if (existingProduct == null || string.IsNullOrEmpty(product.TovarImage))
                    {
                        product.TovarImage = "picture.png";
                    }

                    db.SaveChanges();

                    string message = existingProduct != null ? "Товар успешно обновлен" : "Товар успешно добавлен";
                    MessageBox.Show(message, "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);

                    ProductWindow mainWindow = new ProductWindow();
                    mainWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FillFields(Tovar tovar)
        {
            try
            {
                article.Text = tovar.TovarArticle;
                name.Text = tovar.TovarName;
                unit.Text = tovar.TovarUnit;
                description.Text = tovar.TovarDescription;
                cost.Text = tovar.TovarCost?.ToString();
                discount.Text = tovar.TovarDiscount?.ToString();
                quantity.Text = tovar.TovarCount?.ToString();

                LoadImageFromResources(tovar.TovarImage);

                if (tovar.TovarCategoryId.HasValue && cbCategories.Items.Count > 0)
                {
                    cbCategories.SelectedIndex = tovar.TovarCategoryId.Value - 1;
                }

                if (tovar.ManufacturerId.HasValue && manufacturer.Items.Count > 0)
                {
                    manufacturer.SelectedIndex = tovar.ManufacturerId.Value - 1;
                }

                if (tovar.SupplierId.HasValue && delivery.Items.Count > 0)
                {
                    delivery.SelectedIndex = tovar.SupplierId.Value - 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при заполнении полей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void backMain_btn_Click(object sender, RoutedEventArgs e)
        {
            ProductWindow productWindow = new ProductWindow();
            productWindow.Show();
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