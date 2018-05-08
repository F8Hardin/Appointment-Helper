using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DoctorHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            time_box_setup();
            dr_finder.Navigate("https://doctor.webmd.com/");
        }

        public void time_box_setup() //set up combo boxes for time
        {
            Hour_Box.SelectedIndex = 0;
            Minute_Box.SelectedIndex = 0;

            Hour_Box.Items.Add("-");
            Minute_Box.Items.Add("--");

            for (int i = 1; i < 13; i++)
            {
                Hour_Box.Items.Add(i);
            }

            for(int i = 0; i < 60; i++)
            {
                if (i < 10)
                    Minute_Box.Items.Add("0" + i.ToString());
                else
                    Minute_Box.Items.Add(i);
            }
        }

        public void Notes_Focus(object sender, RoutedEventArgs e) //removes text from appointment_notes textbox when clicked
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= Notes_Focus;
        }
    }
}
