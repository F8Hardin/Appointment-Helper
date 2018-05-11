using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorHelper
{
    class Family
    {
        public string name { get; set; }
        public List<Medications> meds = new List<Medications>();
        public string display_meds { get; set; } //used when displaying med names, when something is removed from the list of medications remove the substring from this string
        public List<Doctors> doctors = new List<Doctors>();
        public string display_doctors { get; set; } //used for displaying doctor names, same as display_meds
    }
}
