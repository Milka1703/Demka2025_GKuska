using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Krasnoyarsk
{
    public partial class CleaningStaffScheduleWindow : Window
    {
        private readonly hotel_managementEntities _context;
        private readonly int _currentUserId;

        public CleaningStaffScheduleWindow(int userId)
        {
            InitializeComponent();
            _context = new hotel_managementEntities();
            _currentUserId = userId;
            LoadSchedule();
        }
        
        private void LoadSchedule()
        {
           
            try
            {
                var schedule = _context.Cleaning_Schedule
                    .Include("Rooms")
                    .Where(cs => cs.user_id == _currentUserId)
                    .Select(cs => new
                    {
                        cs.id,
                        cs.cleaning_date,
                        cs.Rooms,
                        cs.status
                    })
                    .ToList();
                

                CleaningScheduleGrid.ItemsSource = schedule;
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var selectedSchedule = button?.DataContext as dynamic;

                if (selectedSchedule == null)
                {
                    MessageBox.Show("Не выбрана запись для изменения статуса.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var scheduleEntry = _context.Cleaning_Schedule.Find(selectedSchedule.id);
                if (scheduleEntry == null)
                {
                    MessageBox.Show("Запись не найдена в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                scheduleEntry.status = "Выполнено";
                _context.SaveChanges();

                LoadSchedule();

                MessageBox.Show("Статус успешно изменен на 'Выполнено'.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}