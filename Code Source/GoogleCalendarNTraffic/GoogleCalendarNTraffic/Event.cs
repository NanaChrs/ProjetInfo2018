using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoogleCalendarNTraffic
{
    public class Event
    {
        public string Nom { get; set; }
        public string DateDebut { get; set; }
        public string DateFin { get; set; }
        public string Lieu { get; set; }
    }

    public class EventnTraffic
    {
        public string Nom { get; set; }
        public string DateDebut { get; set; }
        public string DateFin { get; set; }
        public string Lieu { get; set; }
        public string TimeTrafficFromHouse { get; set; }
    }
}
