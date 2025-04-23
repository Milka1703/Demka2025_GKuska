using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Krasnoyarsk
{
    /// <summary>
    /// Interaction logic for CreateBookingWindow.xaml
    /// </summary>
    public partial class CreateBookingWindow : Window
    {
        private readonly hotel_managementEntities _context;

        public CreateBookingWindow()
        {
            InitializeComponent();
            _context = new hotel_managementEntities();
            LoadRooms();
        }

        private void LoadRooms()
        {
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
                MessageBox.Show("Нет доступных номеров.");
            }
        }

        private void SaveBooking_Click(object sender, RoutedEventArgs e)
        {
            var guestLastName = GuestLastNameTextBox.Text.Trim();
            var guestFirstName = GuestFirstNameTextBox.Text.Trim();
            var guestDocumentNumber = GuestDocumentNumberTextBox.Text.Trim();
            var guestEmail = EmailTextBox.Text.Trim();
            var guestPhone = PhoneTextBox.Text.Trim();

            var selectedRoomId = RoomComboBox.SelectedValue as int?;
            var checkInDate = CheckInDatePicker.SelectedDate;
            var checkOutDate = CheckOutDatePicker.SelectedDate;

            if (string.IsNullOrWhiteSpace(guestLastName) || string.IsNullOrWhiteSpace(guestFirstName) ||
                string.IsNullOrWhiteSpace(guestDocumentNumber) ||
                selectedRoomId == null || !checkInDate.HasValue || !checkOutDate.HasValue)
            {
                MessageBox.Show("Заполните все поля.");
                return;
            }

            if (checkInDate.Value >= checkOutDate.Value)
            {
                MessageBox.Show("Дата выезда должна быть позже даты заезда.");
                return;
            }

            try
            {
                var selectedRoom = _context.Rooms.FirstOrDefault(r => r.id == selectedRoomId.Value);
                if (selectedRoom == null)
                {
                    MessageBox.Show("Выбранный номер не найден.");
                    return;
                }

                var nightsCount = (checkOutDate.Value - checkInDate.Value).Days;

                var totalPrice = nightsCount * selectedRoom.price_per_night;

                var newGuest = new Guests
                {
                    first_name = guestFirstName,
                    last_name = guestLastName,
                    email = guestEmail,
                    phone = guestPhone,
                    document_number = guestDocumentNumber
                };

                _context.Guests.Add(newGuest);
                _context.SaveChanges();

                var newReservation = new Reservations
                {
                    guest_id = newGuest.id,
                    room_id = selectedRoomId.Value,
                    check_in_date = checkInDate.Value,
                    check_out_date = checkOutDate.Value,
                    total_price = totalPrice, 
                    status = "Подтверждено"
                };

                _context.Reservations.Add(newReservation);
                _context.SaveChanges();

                MessageBox.Show($"Бронирование успешно создано. Общая стоимость: {totalPrice} руб.");
                this.Close();
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(validationError => validationError.ValidationErrors
                        .Select(error => $"Сущность: {validationError.Entry.Entity.GetType().Name}, " +
                                         $"Поле: {error.PropertyName}, " +
                                         $"Ошибка: {error.ErrorMessage}"))
                    .ToList();

                MessageBox.Show(string.Join(Environment.NewLine, errorMessages));
            }
            catch (Exception ex)
            {
                string errorMessage = $"Ошибка при создании бронирования: {ex.Message}";

                Exception currentEx = ex;
                while (currentEx != null)
                {
                    errorMessage += $"\nВнутренняя ошибка: {currentEx.Message}";
                    currentEx = currentEx.InnerException;
                }

                MessageBox.Show(errorMessage);
            }
        }
    }
}