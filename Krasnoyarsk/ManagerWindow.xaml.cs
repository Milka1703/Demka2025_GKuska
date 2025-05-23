﻿using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Krasnoyarsk
{
    /// <summary>
    /// Interaction logic for ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        public ManagerWindow()
        {
            InitializeComponent();

            GuestsFrame.Navigate(new GuestsPage());
            RoomsFrame.Navigate(new RoomsPage());
            BookingsFrame.Navigate(new BookingPage());
            CleaningManagementFrame.Navigate(new CleaningManagementPage());
        }
    }
}
