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
using System.IO;

//files are called doctors.txt, family.txt, and appointments.txt

namespace DoctorHelper
{
    public partial class MainWindow : Window
    {
        List<Family> members = new List<Family>();
        List<Medications> new_meds_list = new List<Medications>();
        List<Doctors> Our_doctors = new List<Doctors>();

        Family fam;
        Doctors doc;
        Medications meds;

        public MainWindow()
        {
            InitializeComponent();
            time_box_setup();
            dr_finder.Navigate("https://doctor.webmd.com/");
            create_files();
            open_family_file();
            open_doctors_file();

            remove_doctor.Visibility = Visibility.Collapsed;
            remove_meds.Visibility = Visibility.Collapsed;
        }

        public void time_box_setup() //set up combo boxes for time in the appoinments tab
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
            save_family_to_file();
            add_family.Content = "Add Family Member";
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

        private void Meds_Click(object sender, RoutedEventArgs e) //adds medications to the display list
        {
            string med = med_name.Text;
            string dosage = dosage_textbox.Text;
            string date = null;
            Medications new_med = new Medications();

            if (date_selected.SelectedDate.HasValue)
            {
                date = date_selected.SelectedDate.Value.ToString("MM/dd/yyyy");
            }
            else
            {
                date = "__/__/____";
            }

            if(dosage_textbox.Text == "")
            {
                new_med.dosage = "~";
            }
            else
            {
                new_med.dosage = dosage;
            }

            if (med.Contains(" "))
            {
                med.Replace(" ", "_");
            }
            new_med.name = med;
            new_med.refill_date = date;

            new_meds_list.Add(new_med);
            Meds.Items.Add(new_med);

            med_name.Text = "";
            dosage_textbox.Text = "";
            date_selected.SelectedDate = null;
        }

        private void create_files() //creates the files needed that store the doctors, family members, and appointments
        {
            if (!File.Exists(@"doctors.txt"))
                File.Create(@"doctors.txt").Close();
            if (!File.Exists(@"family.txt"))
                File.Create(@"family.txt").Close();
            if (!File.Exists(@"appoinments.txt"))
                File.Create(@"appointments.txt").Close();
        }

        private void save_family_to_file() //saves the family members to a file
        {
            members.Sort((x, y) => x.name.CompareTo(y.name));

            using (System.IO.StreamWriter myFile = new System.IO.StreamWriter(@"C:\Users\Fate\Desktop\Programs\Visual Studio\DoctorHelper\DoctorHelper\bin\Debug\family.txt"))
            {
                foreach (var Family in members)
                {
                    myFile.WriteLine(Family.name);

                    foreach (var Medications in Family.meds)
                    {
                        myFile.WriteLine("=" + Medications.name + " " + Medications.dosage + " " + Medications.refill_date); //medications are stored with a "-" so when reading in a file, use "while(myFile.ReadLine.Contains("-")" or something
                    }

                    foreach (var Doctors in Family.doctors) //same as medicaitons except stored with a "+"
                    {
                        myFile.WriteLine("+" + Doctors.name + " " + Doctors.office + " " + Doctors.phone);
                    }
                }
            }
        }

        private void open_family_file() //opens the file of family members and adds them to the combo box in appoinments tab and the Listbox in family tab
        {
            using (var myFile = new System.IO.StreamReader(@"C:\Users\Fate\Desktop\Programs\Visual Studio\DoctorHelper\DoctorHelper\bin\Debug\family.txt"))
            {
                string line = myFile.ReadLine();

                while(line != null)
                {
                    fam = new Family();
                    fam.name = line;
                    line = myFile.ReadLine();

                    while(line != null && line.Contains("=") != false)
                    {
                        meds = seperate_meds(line);
                        fam.meds.Add(meds);
                        fam.display_meds = fam.display_meds + meds.name + ", ";
                        line = myFile.ReadLine();
                    }

                    while(line != null && line.Contains("+") != false)
                    {
                        doc = seperate_doctors(line);
                        fam.doctors.Add(doc);
                        fam.display_doctors = fam.display_doctors + doc.name + ", ";
                        line = myFile.ReadLine();
                    }

                    members.Add(fam);
                    Family_ListBox.Items.Add(fam);
                }
                myFile.Close();
            }
        }

        private Medications seperate_meds(string line) //seperates the string of medicine into objects when being read in from a file
        {
            Medications medicine = new Medications();

            line = line.Remove(0, 1);
            string[] split_meds = line.Split(' ');

            medicine.name = split_meds[0];
            medicine.dosage = split_meds[1];
            medicine.refill_date = split_meds[2];

            return medicine;
        }

        private Doctors seperate_doctors(string line) //seperates the string of doctors when read in
        {
            Doctors doctor = new Doctors();

            line = line.Remove(0, 1);
            string[] split_doctors = line.Split(' ');

            doctor.name = split_doctors[0];
            doctor.office = split_doctors[1];
            doctor.phone = split_doctors[2];

            return doctor;
        }

        private void Family_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            edit_family.Content = "Edit Family Member";
            remove_family.Content = "Remove Family Member";
        }

        private void edit_family_Click(object sender, RoutedEventArgs e) //allows the user to edit a selected family member
        {
            if(edit_family.Content != "Edit Family Member")
            {
                MessageBox.Show("No family member selected.");
                return;
            }

            MessageBox.Show("Remember to save changes.");

            Meds.SelectedIndex = -1;
            selected_doctors.SelectedIndex = -1;

            add_family.Content = "Save Changes";
            fam = (Family)Family_ListBox.SelectedItem;
            members.Remove(fam);
            Family_ListBox.Items.Remove(fam);

            fam_name.Text = fam.name;

            foreach(var Doctor in fam.doctors)
            {
                selected_doctors.Items.Add(Doctor);
            }

            foreach(var Medications in fam.meds)
            {
                Meds.Items.Add(Medications);
            }
        }

        private void remove_family_Click(object sender, RoutedEventArgs e) //removes the selected family member from the program
        {
            if(remove_family.Content != "Remove Family Member")
            {
                MessageBox.Show("No family member selected.");
                return;
            }

            Family member = (Family)Family_ListBox.SelectedItem;
            MessageBoxResult result = MessageBox.Show("Are you sure you want to remove " + member.name + "?", "Warning", MessageBoxButton.YesNo);

            if(result == MessageBoxResult.Yes)
            {
                Family_ListBox.Items.Remove(member);
                members.Remove(member);
            }

            else
            {
                return;
            }

            save_family_to_file();
        }

        private void remove_meds_Click(object sender, RoutedEventArgs e) //removes the selected medication from the medications box for adding family members
        {
            meds = (Medications)Meds.SelectedItem;
            Meds.Items.Remove(meds);
            remove_meds.Visibility = Visibility.Collapsed;
        }

        private void Meds_SelectionChanged(object sender, SelectionChangedEventArgs e) //makes the "Remove Medication" button pop up when a medication is selected
        {
            remove_meds.Visibility = Visibility.Visible;
        }

        private void selected_doctors_SelectionChanged(object sender, SelectionChangedEventArgs e) //makes the "Remove Doctor" button pop up when a doctor is selected
        {
            remove_doctor.Visibility = Visibility.Visible;
        }

        private void remove_doctor_Click(object sender, RoutedEventArgs e) //removes a doctor from the box for adding family members
        {
            doc = (Doctors)selected_doctors.SelectedItem;
            selected_doctors.Items.Remove(doc);
            remove_doctor.Visibility = Visibility.Collapsed;
        }

        private void open_doctors_file() //opens the file containing all of the doctors and adds the doctors to the combo box in family and appointments
        {
            using (var MyFile = new System.IO.StreamReader(@"C:\Users\Fate\Desktop\Programs\Visual Studio\DoctorHelper\DoctorHelper\bin\Debug\doctors.txt"))
            {
                string line;

                while ((line = MyFile.ReadLine()) != null)
                {
                    doc = new Doctors();

                    doc.name = line;
                    line = MyFile.ReadLine();
                    doc.office = line;
                    line = MyFile.ReadLine();
                    doc.phone = line;

                    Our_doctors.Add(doc);
                    doctors_to_select.Items.Add(doc);
                    doctor_info_ListView.Items.Add(doc);
                }
            }
        }

        private void save_doctors_file() //saves doctors to the file
        {
            Our_doctors.Sort((x, y) => x.name.CompareTo(y.name));

            using (var MyFile = new System.IO.StreamWriter(@"C:\Users\Fate\Desktop\Programs\Visual Studio\DoctorHelper\DoctorHelper\bin\Debug\doctors.txt"))
            {
                foreach(var Doctor in Our_doctors)
                {
                    MyFile.WriteLine(Doctor.name);
                    MyFile.WriteLine(Doctor.office);
                    MyFile.WriteLine(Doctor.phone);
                }
            }
        }

        private void Add_doctor_click(object sender, RoutedEventArgs e) //adds the doctor
        {
            doc = new Doctors();

            if(doctor_name_box.Text == "")
            {
                MessageBox.Show("No name given.", "Error");
                return;
            }
            if(doctor_office_box.Text == "")
                doctor_office_box.Text = "-";
            if (doctor_phone_box.Text == "")
                doctor_phone_box.Text = "-";

            doc.name = doctor_name_box.Text;
            doc.phone = doctor_phone_box.Text;
            doc.office = doctor_office_box.Text;

            doctor_name_box.Text = "";
            doctor_phone_box.Text = "";
            doctor_office_box.Text = "";

            doctor_info_ListView.Items.Add(doc);
            doctors_to_select.Items.Add(doc);
            Our_doctors.Add(doc);
            save_doctors_file();
        }
    }
}
