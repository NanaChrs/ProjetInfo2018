using System;

namespace Brain
{
    public class ParametresReveil
    {
        public bool IsActive { get; set; }
        public bool BigSleeper { get; set; }
        public bool ManualMode { get; set; }
        public int ManualAlarmHour { get; set; }
        public int ManualAlarmMinute { get; set; }
    }

    public class Alarme
    {
        public bool IsRinging { get; set; }
        public int AlarmHour { get; set; }
        public int AlarmMinutes { get; set; }
        public string TimeTraffic { get; set; }
        public string TimeFirstEvent { get; set; }

    }

    public class Infos
    {
        public string Traffic { get; set; }
        public string Meteo { get; set; }

    }

    public class Hour
    {
        public int hour { get; set; }
        public int minutes { get; set; }
    }
    public class EventnTraffic
    {
        public string Nom { get; set; }
        public string DateDebut { get; set; }
        public string DateFin { get; set; }
        public string Lieu { get; set; }
        public string TimeTrafficFromHouse { get; set; }
    }

    public class TimeAndPlace
    {
        public DateTime Time { get; set; }
        public string Place { get; set; }
        public TimeSpan TimeTraffic { get; set; }
    }

    public class Coordonnes
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

    public class Route
    {
        public string Path { get; set; }
        public int Time { get; set; }
        public int RealTime { get; set; }
        public string RouteType { get; set; }

    }

}



