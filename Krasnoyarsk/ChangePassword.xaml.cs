using System;
using System.Windows;
using System.Linq;

namespace Krasnoyarsk
{
    /// <summary>
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        private readonly int _userId;

        public ChangePassword(int userId)
        {
            InitializeComponent();
            _userId = userId;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string currentPassword = txtCurrentPassword.Password;
            string newPassword = txtNewPassword.Password;
            string confirmNewPassword = txtConfirmNewPassword.Password;

            if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmNewPassword))
            {
                MessageBox.Show("Все поля обязательны к заполнению.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (newPassword != confirmNewPassword)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                using (var context = new hotel_managementEntities())
                {
                    var user = context.Users.FirstOrDefault(u => u.id == _userId);

                    if (user == null)
                    {
                        MessageBox.Show("Пользователь не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (user.password != currentPassword)
                    {
                        MessageBox.Show("Неверный пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    user.password = newPassword;
                    user.isFirstLogin = false;

                    context.SaveChanges();
                    MessageBox.Show("пароль успешно изменён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении пароля: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
