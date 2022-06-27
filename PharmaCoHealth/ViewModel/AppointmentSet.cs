using PharmaCoHealth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmaCoHealth.ViewModel
{
    public class AppointmentSet
    {
        public IEnumerable<Doctor> doctors { get; set; }
        public IEnumerable<Specialize> specialize { get; set; }
    }
}