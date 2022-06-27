using PharmaCoHealth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmaCoHealth.ViewModel
{
    public class CheckingAppointment
    {
        public List<string> patient { get; set; }
        public List<string> schedule { get; set; }
    }
}