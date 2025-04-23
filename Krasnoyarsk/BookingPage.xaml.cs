using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace Krasnoyarsk
{
    /// <summary>
    /// Interaction logic for BookingPage.xaml
    /// </summary>
    public partial class BookingPage : Page
    {
        public BookingPage()
        {
            InitializeComponent();
            LoadBookings();
        }

        private void LoadBookings()
        {
            try
            {
                using (var context = new hotel_managementEntities())
                {
                    var bookings = context.Reservations
                        .Include("Guests")
                        .Include("Rooms")
                        .ToList();

                    if (bookings.Count == 0)
                    {
                        MessageBox.Show("Нет доступных данных для отображения.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    var selectedBookings = bookings.Select(r => new
                    {
                        r.id,
                        FullName = r.Guests != null ? $"{r.Guests.first_name} {r.Guests.last_name}" : "Нет данных",
                        RoomNumber = r.Rooms != null ? r.Rooms.number.ToString() : "Нет данных",
                        r.check_in_date,
                        r.check_out_date,
                        r.total_price,
                        r.status
                    }).ToList();

                    BookingsDataGrid.ItemsSource = selectedBookings;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CreateBookingWindow createBookingWindow = new CreateBookingWindow();
            createBookingWindow.ShowDialog();
        }
    }
}
