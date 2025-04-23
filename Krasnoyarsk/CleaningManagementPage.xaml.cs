using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Krasnoyarsk
{
    public partial class CleaningManagementPage : Page
    {
        private readonly hotel_managementEntities _context;

        public CleaningManagementPage()
        {
            InitializeComponent();
            _context = new hotel_managementEntities(); 
            LoadData(); 
            LoadRooms(); 
        }

        private void LoadData()
        {
            try
            {
                // Загрузка сотрудников
                var employees = _context.Users
                    .Where(u => u.role == "cleaning_staff") 
                    .ToList() 
                    .Select(u => new { u.id, FullName = $"{u.lastname} {u.firstname}" }) 
                    .ToList();
                CleaningOfficerComboBox.ItemsSource = employees;

                // Загрузка расписания уборки
                var cleaningSchedule = _context.Cleaning_Schedule
                    .Include("Rooms") 
                    .Include("Users")
                    .ToList() 
                    .Select(cs => new
                    {
                        cs.cleaning_date,
                        RoomNumber = cs.Rooms?.number.ToString() ?? "Нет данных",
                        UserFullName = $"{cs.Users?.lastname ?? ""} {cs.Users?.firstname ?? ""}",
                        cs.status
                    })
                    .ToList();
                CleaningScheduleGrid.ItemsSource = cleaningSchedule;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRooms()
        {
            try
            {
                // Загрузка доступных комнат
                var allAvailableRooms = _context.Rooms
                    .Where(r => r.status == "Свободен") 
                    .Select(r => new { r.id, r.number })
                    .ToList();

                var uniqueRooms = new HashSet<int>(); 
                var result = new List<dynamic>();

                foreach (var room in allAvailableRooms)
                {
                    if (uniqueRooms.Add(room.number)) 
                    {
                        result.Add(new { id = room.id, number = room.number });
                    }
                }

                if (result.Any())
                {
                    RoomComboBox.ItemsSource = result; 
                    RoomComboBox.DisplayMemberPath = "number"; 
                    RoomComboBox.SelectedValuePath = "id"; 
                }
                else
                {
                    MessageBox.Show("Нет доступных номеров.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке комнат: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AssignButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получение выбранных значений из ComboBox
                var selectedEmployee = CleaningOfficerComboBox.SelectedItem as dynamic;
                var selectedRoom = RoomComboBox.SelectedItem as dynamic;

                if (selectedEmployee == null || selectedRoom == null || CleaningDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Создание новой записи в расписании
                var newCleaningSchedule = new Cleaning_Schedule
                {
                    room_id = selectedRoom.id,
                    user_id = selectedEmployee.id,
                    cleaning_date = CleaningDatePicker.SelectedDate.Value,
                    status = "Запланировано" 
                };

                _context.Cleaning_Schedule.Add(newCleaningSchedule);
                _context.SaveChanges();

                LoadData();

                MessageBox.Show("Уборка успешно назначена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Произошла ошибка: {ex.Message}";

                Exception currentEx = ex;
                while (currentEx != null)
                {
                    errorMessage += $"\nВнутренняя ошибка: {currentEx.Message}";
                    currentEx = currentEx.InnerException;
                }

                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}