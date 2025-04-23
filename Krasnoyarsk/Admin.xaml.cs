using System.Collections.Generic;
using System.Data.Entity;
using System.Windows;

namespace Krasnoyarsk
{
    /// <summary>
    /// Interaction logic for Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        public Admin()
        {
            InitializeComponent();
            LoadUsers();

        }

        private async void LoadUsers()
        {
            using (var context = new hotel_managementEntities())
            {
                var users = await context.Users.ToListAsync();
                Users.ItemsSource = users;
            }
        }

        private async void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var newUserWindow = new AddUserWindow();
            if (newUserWindow.ShowDialog() == true && newUserWindow.NewUser != null)
            {
                var newUser = newUserWindow.NewUser;

                using (var context = new hotel_managementEntities())
                {
                    if (await context.Users.AnyAsync(u => u.username == newUser.username))
                    {
                        MessageBox.Show("Пользователь с таким именем уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        try
                        {
                            context.Users.Add(newUser);
                            await context.SaveChangesAsync();
                        }
                        catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                        {
                            MessageBox.Show($"Ошибка при сохранении данных: {ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        LoadUsers();
                        MessageBox.Show("Пользователь успешно добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Добавление пользователя отменено.", "Отмена", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void UnlockUser_Click(object sender, RoutedEventArgs e)
        {
            if (Users.SelectedItem is Users selectedUser)
            {
                using (var context = new hotel_managementEntities())
                {
                    var users = await context.Users.FindAsync(selectedUser.id);

                    if (users != null)
                    {
                        users.IsLocked = false;
                        users.LastLoginDate = null;
                        await context.SaveChangesAsync();
                        LoadUsers();
                        MessageBox.Show("Пользователь разблокирован", "Ура!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя", "грусть", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new hotel_managementEntities())
            {
                foreach (var user in Users.ItemsSource as IEnumerable<Users>)
                {
                    var existingUser = await context.Users.FindAsync(user.id);
                    if (existingUser != null)
                    {
                        existingUser.lastname = user.lastname;
                        existingUser.firstname = user.firstname;
                        existingUser.role = user.role;
                        existingUser.username = user.username;
                        existingUser.IsLocked = user.IsLocked;

                    }
                }
                await context.SaveChangesAsync();
                LoadUsers();
                MessageBox.Show("Изменения успешно сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
