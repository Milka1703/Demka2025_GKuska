using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
//using System.Security.Cryptography;

namespace Krasnoyarsk
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

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль");
                return;
            }

            using (var context = new hotel_managementEntities())
            {
                var user = await context.Users
                    .Where(u => u.username == username)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    MessageBox.Show("Неправильный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (user.IsLocked.HasValue && user.IsLocked.Value)
                {
                    MessageBox.Show("Вы заблокированы, обратитесь, пожалуйста к администратору.", "Доступ запрещен.", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (user.LastLoginDate.HasValue && (DateTime.Now - user.LastLoginDate.Value).TotalDays > 30 && user.role != "Admin")
                {
                    user.IsLocked = true;
                    await context.SaveChangesAsync();
                    MessageBox.Show("Ваша учетная запись заблокирована из-за длительного отсутствия", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (user.password == password)
                {
                    user.LastLoginDate = DateTime.Now;
                    user.FailedLoginAttempts = 0;
                    await context.SaveChangesAsync();
                    MessageBox.Show("Вы успешно авторизовались", "Добро пожаловать!", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (user.isFirstLogin.HasValue && user.isFirstLogin.Value)
                    {
                        ChangePassword changePasswordWindow = new ChangePassword(user.id);
                        changePasswordWindow.Owner = this;
                        changePasswordWindow.ShowDialog();
                    }
                    else
                    {
                        if (user.role == "Admin")
                        {
                            Admin adminWindow = new Admin();
                            adminWindow.Show();
                        }
                        else if (user.role == "Management")
                        {
                            ManagerWindow managerWindow = new ManagerWindow();
                            managerWindow.Show();
                        }
                        else if (user.role == "cleaning_staff")
                        {
                            CleaningStaffScheduleWindow cleaningStaffWin = new CleaningStaffScheduleWindow(user.id);
                            cleaningStaffWin.Show();
                        }
                        else
                        {
                            MainWindow userWindow = new MainWindow();
                            userWindow.Show();
                        }

                        this.Close();
                    }
                }
                else
                {
                    user.FailedLoginAttempts++;
                    if (user.FailedLoginAttempts == 3)
                    {
                        user.IsLocked = true;
                        MessageBox.Show("Вы заблокированы после 3 неудачных попыток входа.", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        int attemptsLeft = 3 - (user.FailedLoginAttempts ?? 0);
                        MessageBox.Show($"Неправильный логин и пароль. Осталось попыток: {attemptsLeft}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
