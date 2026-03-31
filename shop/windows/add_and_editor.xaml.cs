using shop.model;
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

namespace shop.windows
{
    /// <summary>
    /// Логика взаимодействия для add_and_editor.xaml
    /// </summary>
    public partial class add_and_editor : Window
    {
        //public add_and_editor()
        //{
        //    InitializeComponent();
        //}

        private shopEntities db = new shopEntities();

        private Product currentProduct;

        private bool isEditMode = false;



        // Свойства для привязки

        public string WindowTitle { get; set; }

        public string Namee { get; set; }

        public string Category { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string Status { get; set; }

        public string Manufacturer { get; set; }

        public string Article { get; set; }

        public string Description { get; set; }

        public decimal? Weight { get; set; }

        public string Size { get; set; }

        public string Color { get; set; }

        public string Material { get; set; }

        public DateTime AddedDate { get; set; }

        public bool IsEditMode => isEditMode;

        // Конструктор для добавления нового товара

        public add_and_editor()

        {

            InitializeComponent();

            DataContext = this;

            isEditMode = false;

            WindowTitle = "Добавление товара";



            // Установка значений по умолчанию

            Price = 0;

            Quantity = 0;

            Status = "В наличии";

            AddedDate = DateTime.Now;



            LoadCategories();

        }



        // Конструктор для редактирования существующего товара

        public add_and_editor(Product product) : this()

        {

            isEditMode = true;

            WindowTitle = "Редактирование товара";

            currentProduct = product;



            // Загружаем данные товара в форму

            LoadProductData();

        }



        // Загрузка категорий в ComboBox

        private void LoadCategories()

        {

            try

            {

                var categories = db.Categories.ToList();

                cmbCategory.Items.Clear();



                foreach (var category in categories)

                {

                    cmbCategory.Items.Add(category.Name);

                }



                if (cmbCategory.Items.Count > 0)

                {

                    cmbCategory.SelectedIndex = 0;

                }

            }

            catch (Exception ex)

            {

                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}",

                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }



        // Загрузка данных товара для редактирования

        private void LoadProductData()

        {

            if (currentProduct == null) return;



            try

            {

                // Заполняем основные поля

                Namee = currentProduct.Name;

                Quantity = currentProduct.Quantity;

                Price = currentProduct.Price;

                AddedDate = currentProduct.AddedDate;

                Status = currentProduct.Status;

                Manufacturer = currentProduct.Manufacturer;

                Article = currentProduct.Article;

                Description = currentProduct.Description;

                Weight = currentProduct.Weight;

                Size = currentProduct.Size;

                Color = currentProduct.Color;

                Material = currentProduct.Material;



                // Загружаем категорию

                if (currentProduct.Category != null)

                {

                    Category = currentProduct.Category.Name;

                    cmbCategory.Text = Category;

                }



                // Обновляем привязки

                txtName.Text = Name;

                txtPrice.Text = Price.ToString();

                txtQuantity.Text = Quantity.ToString();

                txtManufacturer.Text = Manufacturer;

                txtArticle.Text = Article;

                txtDescription.Text = Description;

                txtWeight.Text = Weight?.ToString() ?? "";

                txtSize.Text = Size;

                txtColor.Text = Color;

                txtMaterial.Text = Material;



                // Устанавливаем выбранный статус в ComboBox

                foreach (ComboBoxItem item in cmbStatus.Items)

                {

                    if (item.Content.ToString() == Status)

                    {

                        cmbStatus.SelectedItem = item;

                        break;

                    }

                }

            }

            catch (Exception ex)

            {

                MessageBox.Show($"Ошибка загрузки данных товара: {ex.Message}",

                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }



        // Проверка валидности данных

        private bool ValidateInput()

        {

            // Проверка наименования

            if (string.IsNullOrWhiteSpace(txtName.Text))

            {

                MessageBox.Show("Введите наименование товара", "Ошибка",

                    MessageBoxButton.OK, MessageBoxImage.Warning);

                txtName.Focus();

                return false;

            }



            // Проверка категории

            if (cmbCategory.SelectedItem == null && string.IsNullOrWhiteSpace(cmbCategory.Text))

            {

                MessageBox.Show("Выберите категорию товара", "Ошибка",

                    MessageBoxButton.OK, MessageBoxImage.Warning);

                cmbCategory.Focus();

                return false;

            }



            // Проверка цены

            decimal price;

            if (!decimal.TryParse(txtPrice.Text, out price) || price < 0)

            {

                MessageBox.Show("Введите корректную цену (положительное число)", "Ошибка",

                    MessageBoxButton.OK, MessageBoxImage.Warning);

                txtPrice.Focus();

                return false;

            }



            // Проверка количества

            int quantity;

            if (!int.TryParse(txtQuantity.Text, out quantity) || quantity < 0)

            {

                MessageBox.Show("Введите корректное количество (целое неотрицательное число)", "Ошибка",

                    MessageBoxButton.OK, MessageBoxImage.Warning);

                txtQuantity.Focus();

                return false;

            }



            // Проверка артикула на уникальность (только для новых товаров или при изменении)

            string article = txtArticle.Text.Trim();

            if (!string.IsNullOrWhiteSpace(article))

            {

                var existingProduct = db.Products.FirstOrDefault(p => p.Article == article);

                if (existingProduct != null && (currentProduct == null || existingProduct.Id != currentProduct.Id))

                {

                    MessageBox.Show("Товар с таким артикулом уже существует", "Ошибка",

                        MessageBoxButton.OK, MessageBoxImage.Warning);

                    txtArticle.Focus();

                    return false;

                }

            }



            return true;

        }



        // Сохранение товара

        private void SaveButton_Click(object sender, RoutedEventArgs e)

        {

            try

            {

                if (!ValidateInput())

                    return;



                // Получаем данные из формы

                string productName = txtName.Text.Trim();

                string categoryName = cmbCategory.Text.Trim();

                decimal price = decimal.Parse(txtPrice.Text);

                int quantity = int.Parse(txtQuantity.Text);

                string status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? GetStatusByQuantity(quantity);



                // Получаем дополнительные данные

                string manufacturer = txtManufacturer.Text.Trim();

                string article = string.IsNullOrWhiteSpace(txtArticle.Text) ? null : txtArticle.Text.Trim();

                string description = txtDescription.Text.Trim();

                decimal? weight = string.IsNullOrWhiteSpace(txtWeight.Text) ? (decimal?)null : decimal.Parse(txtWeight.Text);

                string size = txtSize.Text.Trim();

                string color = txtColor.Text.Trim();

                string material = txtMaterial.Text.Trim();



                // Находим или создаем категорию

                var category = db.Categories.FirstOrDefault(c => c.Name == categoryName);

                if (category == null)

                {

                    // Если категории нет, создаем новую

                    category = new Category

                    {

                        Name = categoryName,

                        Description = null

                    };

                    db.Categories.Add(category);

                    db.SaveChanges();

                }



                if (isEditMode && currentProduct != null)

                {

                    // Редактирование существующего товара

                    currentProduct.Name = productName;

                    currentProduct.CategoryId = category.Id;

                    currentProduct.Price = price;

                    currentProduct.Quantity = quantity;

                    currentProduct.Status = status;

                    currentProduct.Manufacturer = manufacturer;

                    currentProduct.Article = article;

                    currentProduct.Description = description;

                    currentProduct.Weight = weight;

                    currentProduct.Size = size;

                    currentProduct.Color = color;

                    currentProduct.Material = material;



                    db.SaveChanges();



                    MessageBox.Show("Товар успешно обновлен", "Успех",

                        MessageBoxButton.OK, MessageBoxImage.Information);

                }

                else

                {

                    // Добавление нового товара

                    var newProduct = new Product

                    {

                        Name = productName,

                        CategoryId = category.Id,

                        Price = price,

                        Quantity = quantity,

                        Status = status,

                        AddedDate = DateTime.Now,

                        Manufacturer = manufacturer,

                        Article = article,

                        Description = description,

                        Weight = weight,

                        Size = size,

                        Color = color,

                        Material = material

                    };



                    db.Products.Add(newProduct);

                    db.SaveChanges();



                    MessageBox.Show("Товар успешно добавлен", "Успех",

                        MessageBoxButton.OK, MessageBoxImage.Information);

                }



                DialogResult = true;

                Close();

            }

            catch (FormatException)

            {

                MessageBox.Show("Ошибка формата данных. Проверьте правильность ввода числовых полей.",

                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);

            }

            catch (Exception ex)

            {

                MessageBox.Show($"Ошибка сохранения товара: {ex.Message}",

                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }



        // Определение статуса по количеству

        private string GetStatusByQuantity(int quantity)

        {

            if (quantity <= 0)

                return "Нет в наличии";

            else if (quantity < 10)

                return "Мало";

            else

                return "В наличии";

        }



        // Отмена

        private void CancelButton_Click(object sender, RoutedEventArgs e)

        {

            var result = MessageBox.Show("Вы уверены, что хотите отменить изменения?",

                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);



            if (result == MessageBoxResult.Yes)

            {

                DialogResult = false;

                Close();

            }

        }



        // Закрытие окна

        private void CloseButton_Click(object sender, RoutedEventArgs e)

        {

            CancelButton_Click(sender, e);

        }



        // Обработка изменения количества для автоматического обновления статуса

        private void txtQuantity_TextChanged(object sender, TextChangedEventArgs e)

        {

            try

            {

                if (int.TryParse(txtQuantity.Text, out int quantity))

                {

                    string newStatus = GetStatusByQuantity(quantity);



                    // Обновляем статус в ComboBox

                    foreach (ComboBoxItem item in cmbStatus.Items)

                    {

                        if (item.Content.ToString() == newStatus)

                        {

                            cmbStatus.SelectedItem = item;

                            break;

                        }

                    }

                }

            }

            catch { }

        }



        // Освобождение ресурсов

        protected override void OnClosed(EventArgs e)

        {

            db?.Dispose();

            base.OnClosed(e);

        }

    


        //private void CloseButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void CancelButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void SaveButton_Click(object sender, RoutedEventArgs e)
        //{

        //}
    }
}
