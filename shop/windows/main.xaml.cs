using shop.windows;
using shop.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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

namespace shop.windows {
    /// <summary>
    /// Логика взаимодействия для main.xaml
    /// </summary>
    public partial class main : Window {

        private shopEntities db = new shopEntities();

        private User currentUser;

        public main() {
            InitializeComponent();
            LoadProducts();
        }
        public main(User user) : this() {

            currentUser = user;

            // Обновляем информацию о пользователе

            txtUserInfo.Text = $"{user.Id} {user.Email}";

        }
        private void LoadProducts()

        {

            try

            {

                var products = db.Products

                    .Include("Category")

                    .Select(p => new

                    {

                        Id = p.Id,

                        Name = p.Name,

                        Category = p.Category.Name,

                        Price = p.Price,

                        Quantity = p.Quantity,

                        Status = p.Quantity > 0 ? "В наличии" : "Нет в наличии",

                        AddedDate = p.AddedDate

                    })

                    .ToList();



                dgProducts.ItemsSource = products;

                txtTotalItems.Text = products.Count.ToString();

            }

            catch (Exception ex)

            {

                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}",

                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }

        // Кнопка "Добавить товар"

        private void btnAdd_Click(object sender, RoutedEventArgs e)

        {

            try

            {

                // Создаем новый продукт

                var newProduct = new Product

                {

                    Name = "Новый товар",

                    CategoryId = 1, // По умолчанию первая категория

                    Price = 0,

                    Quantity = 0,

                    AddedDate = DateTime.Now

                };



                db.Products.Add(newProduct);

                db.SaveChanges();



                // Переименовываем товар с учетом ID

                newProduct.Name = $"Товар {newProduct.Id}";

                db.SaveChanges();



                LoadProducts();

                MessageBox.Show("Товар успешно добавлен", "Информация",

                    MessageBoxButton.OK, MessageBoxImage.Information);

            }

            catch (Exception ex)

            {

                MessageBox.Show($"Ошибка добавления товара: {ex.Message}",

                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }



        // Кнопка "Редактировать"

        private void btnEdit_Click(object sender, RoutedEventArgs e)

        {

            try

            {

                // Получаем выбранный товар

                dynamic selectedProduct = dgProducts.SelectedItem;

                if (selectedProduct == null)

                {

                    MessageBox.Show("Выберите товар для редактирования",

                        "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);

                    return;

                }



                int productId = selectedProduct.Id;

                var product = db.Products.Find(productId);



                if (product != null)

                {

                    // Открываем окно редактирования (нужно создать отдельно)

                    // EditProductWindow editWindow = new EditProductWindow(product);

                    // editWindow.ShowDialog();



                    // Временное решение - просто показываем сообщение

                    MessageBox.Show($"Редактирование товара: {product.Name}",

                        "Редактирование", MessageBoxButton.OK, MessageBoxImage.Information);



                    LoadProducts();

                }

            }

            catch (Exception ex)

            {

                MessageBox.Show($"Ошибка редактирования: {ex.Message}",

                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }



        // Кнопка "Удалить"

        private void btnDelete_Click(object sender, RoutedEventArgs e)

        {

            try

            {

                dynamic selectedProduct = dgProducts.SelectedItem;

                if (selectedProduct == null)

                {

                    MessageBox.Show("Выберите товар для удаления",

                        "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);

                    return;

                }



                var result = MessageBox.Show("Вы действительно хотите удалить выбранный товар?",

                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);



                if (result == MessageBoxResult.Yes)

                {

                    int productId = selectedProduct.Id;

                    var product = db.Products.Find(productId);



                    if (product != null)

                    {

                        db.Products.Remove(product);

                        db.SaveChanges();

                        LoadProducts();



                        MessageBox.Show("Товар успешно удален", "Информация",

                            MessageBoxButton.OK, MessageBoxImage.Information);

                    }

                }

            }

            catch (Exception ex)

            {

                MessageBox.Show($"Ошибка удаления: {ex.Message}",

                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }



        // Поиск товаров

        private void txtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)

        {

            try

            {

                string searchText = txtSearch.Text.ToLower();



                if (string.IsNullOrWhiteSpace(searchText) || searchText == "поиск товаров...")

                {

                    LoadProducts();

                    return;

                }



                var filteredProducts = db.Products

                    .Include("Category")

                    .Where(p => p.Name.ToLower().Contains(searchText) ||

                               p.Category.Name.ToLower().Contains(searchText))

                    .Select(p => new

                    {

                        Id = p.Id,

                        Name = p.Name,

                        Category = p.Category.Name,

                        Price = p.Price,

                        Quantity = p.Quantity,

                        Status = p.Quantity > 0 ? "В наличии" : "Нет в наличии",

                        AddedDate = p.AddedDate

                    })

                    .ToList();



                dgProducts.ItemsSource = filteredProducts;

                txtTotalItems.Text = filteredProducts.Count.ToString();

            }

            catch (Exception)

            {

                // Игнорируем ошибки поиска

                LoadProducts();

            }

        }



        // Обработка двойного клика по строке DataGrid

        private void dgProducts_MouseDoubleClick(object sender, MouseButtonEventArgs e)

        {

            btnEdit_Click(sender, null);

        }



        // Кнопка "Выйти"

        private void btnLogout_Click(object sender, RoutedEventArgs e)

        {

            var result = MessageBox.Show("Вы действительно хотите выйти из системы?",

                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);



            if (result == MessageBoxResult.Yes)

            {

                // Открываем окно авторизации

                authorization loginWindow = new authorization();

                loginWindow.Show();



                // Закрываем текущее окно

                this.Close();

            }

        }



        // Пагинация: кнопки страниц

        private void btnPage1_Click(object sender, RoutedEventArgs e)

        {

            LoadProducts(); // В реальном проекте здесь должна быть загрузка 1-й страницы

        }



        private void btnPage2_Click(object sender, RoutedEventArgs e)

        {

            // Здесь логика для загрузки 2-й страницы

        }



        private void btnPage3_Click(object sender, RoutedEventArgs e)

        {

            // Здесь логика для загрузки 3-й страницы

        }



        private void btnPrevPage_Click(object sender, RoutedEventArgs e)

        {

            // Логика для предыдущей страницы

        }



        private void btnNextPage_Click(object sender, RoutedEventArgs e)

        {

            // Логика для следующей страницы

        }



        // Обработка закрытия окна

        protected override void OnClosed(EventArgs e)

        {

            db?.Dispose();

            base.OnClosed(e);

        }

        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text == "Поиск товаров...") txtSearch.Text = "";
        }
        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text)) txtSearch.Text = "Поиск товаров...";
        }
    }
}
