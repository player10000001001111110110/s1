using shop.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для authorization.xaml
    /// </summary>
    public partial class authorization : Window {

        private shopEntities db = new shopEntities();

        public authorization() {
            InitializeComponent();
        }
        private void ShowError(string message) {
            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;
        }

        private void txtUsername(object sender, TextChangedEventArgs e)  {

        }
            
        private void btnLogin_Click(object sender, RoutedEventArgs e) {
            // Проверка полей
            if (string.IsNullOrWhiteSpace(txtUsername_.Text)) {
                ShowError("Введите email");
                return;
            }

            if (txtPassword.Password == "")
            {
                ShowError("Введите пароль");
                return;
            }

            try
            {
                // Ищем пользователя в базе
                var user = db.Users.FirstOrDefault(u => u.Email == txtUsername_.Text);

                if (user == null)
                {
                    ShowError("Пользователь не найден");
                    return;
                }

                // Проверяем пароль
                if (user.PasswordHash == txtPassword.Password)
                {
                    // Успешный вход
                    MessageBox.Show($"Добро пожаловать, {user.Email}!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Открываем главное окно
                    main mainWindow = new main(user);
                    mainWindow.Show();

                    // Закрываем окно авторизации
                    this.Close();
                }
                else
                {
                    ShowError("Неверный пароль");
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка подключения к БД: " + ex.Message);
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e) {

        }
    }
}
