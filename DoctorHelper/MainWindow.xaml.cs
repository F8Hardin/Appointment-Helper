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
        List<Family> members = new List<Family>();
        Family fam;
        Doctors doc;
        Medications meds;

        public MainWindow()
        {
            InitializeComponent();
            time_box_setup();
            dr_finder.Navigate("https://doctor.webmd.com/");

            Doctors doc = new Doctors();
            doc.name = "Who";
            doc.office = "TARDIS";
            doctors_to_select.Items.Add(doc);
            Doctors doc2 = new Doctors();
            doc2.name = "Thomas";
            doc2.office = "Hell";
            doctors_to_select.Items.Add(doc2);
            Medications med = new Medications();
            med.name = "Viagra";
            Meds.Items.Add(med);
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

        public void Add_Family_Click(object sender, RoutedEventArgs e) //all the logic for adding a family member
        {
            fam = new Family();

            if(fam_name.Text == "") //if no name is given, return
            {
                MessageBox.Show("No name given.");
                return;
            }

            fam.name = fam_name.Text;

            if(selected_doctors.Items.Count != 0) //loops through selected_doctors and adds them to the family member's list of doctors
            {
                for(int i = 0; i < selected_doctors.Items.Count; i++)
                {
                    doc = new Doctors();
                    doc = selected_doctors.Items[i] as Doctors;
                    fam.doctors.Add(doc);
                    fam.display_doctors += doc.name + ", ";
                }
            }

            if(Meds.Items.Count != 0) //if the family member has medications, they are added to the family member's list of medications
            {
                for(int i = 0; i < Meds.Items.Count; i++)
                {
                    meds = new Medications();
                    meds = Meds.Items[i] as Medications;
                    fam.meds.Add(meds);
                    fam.display_meds += meds.name + ", ";
                }
            }

            Family_ListBox.Items.Add(fam);
            members.Add(fam);
            Meds.Items.Clear();
            selected_doctors.Items.Clear();
            fam_name.Text = "";
        }

        private void Select_Doctor_Click(object sender, RoutedEventArgs e) //when clicked, the Doctor from doctors_to_select selected is added to selected_doctors that will be part of that family member's list of doctors
        {
            if (doctors_to_select.SelectedItems.Count > 0 && !selected_doctors.Items.Contains(doctors_to_select.SelectedItem))
            {
                selected_doctors.Items.Add(doctors_to_select.SelectedItem);
            }

            else
                return;
        }
    }
}
