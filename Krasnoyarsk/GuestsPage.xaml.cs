using System.Linq;
using System.Windows.Controls;

namespace Krasnoyarsk
{
    /// <summary>
    /// Interaction logic for GuestsPage.xaml
    /// </summary>
    public partial class GuestsPage : Page
    {
        private hotel_managementEntities _context;

        public GuestsPage()
        {
            InitializeComponent();
            _context = new hotel_managementEntities();
            LoadData();
        }

        private void LoadData()
        {
            var guests = _context.Guests.ToList();
            GuestsDataGrid.ItemsSource = guests;
        }
    }
}
