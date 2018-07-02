using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;
using System.Timers;

namespace Brain
{
    public partial class Program : PackageBase
    {
        public TimeAndPlace GetWakeUpHour()
        {
            var TimeReveil = new TimeAndPlace();
            List<EventnTraffic> ListEvent = Events.DynamicValue.ToObject<List<EventnTraffic>>();
            foreach (var evenement in ListEvent)
            {
                if (DateTime.Now < DateTime.Parse(evenement.DateDebut))
                {
                    PackageHost.WriteInfo("Le prochain évènement est le {0}", evenement.DateDebut.ToString());
                    TimeReveil.Time = DateTime.Parse(evenement.DateDebut);
                    try
                    {
                        TimeReveil.TimeTraffic = TimeSpan.Parse(evenement.TimeTrafficFromHouse.ToString());
                        TimeReveil.Place = evenement.Lieu;
                    }
                    catch
                    {
                        var title = "Absence lieu pour le lendemain";
                        var body = "Aucun lieu n'a été enregistré pour demain, le temps de trajet ne sera pas pris en compte";
                        PackageHost.SendMessage(MessageScope.Create("PushBullet"), "PushNote", new object[] { title as System.String, body as System.String });
                        TimeReveil.TimeTraffic = new TimeSpan();
                    }
                    return TimeReveil;
                }
            }
            return TimeReveil;

        }

        public void ModeManuel()
        {
            if(int.Parse(ParametresServeur.DynamicValue.AlarmHour.ToString()) > 0 && int.Parse(ParametresServeur.DynamicValue.AlarmMinutes.ToString()) > 0)
            { 
                PackageHost.WriteInfo("Mode de réveil = 1");
                Alarme NewAlarm = new Alarme();
                NewAlarm.IsRinging = false;
                NewAlarm.AlarmHour = int.Parse(ParametresServeur.DynamicValue.ManualAlarmHour.ToString());
                NewAlarm.AlarmMinutes = int.Parse(ParametresServeur.DynamicValue.ManualAlarmMinute.ToString());
                var title = "Alarme en mode manuel";
                var body = string.Format("L'alarme sonnera à {0}:{1}",NewAlarm.AlarmHour.ToString("00"), NewAlarm.AlarmMinutes.ToString("00"));
                PackageHost.SendMessage(MessageScope.Create("PushBullet"), "PushNote", new object[] { title as System.String, body as System.String });
                ChangeAlarm(NewAlarm);
                return;

            }
            else
            {
                var title = "Erreur mode manuel";
                var body = string.Format("L'heure entrée pour le mode manuel n'est pas correct !");
                PackageHost.SendMessage(MessageScope.Create("PushBullet"), "PushNote", new object[] { title as System.String, body as System.String });
            }
        }

        public void ModeAuto()
        {
            TimeAndPlace WakeUp = GetWakeUpHour();
            TimeSpan prep_time = new TimeSpan();
            string title, body;
            try
            {
                prep_time = new TimeSpan(0, int.Parse(PackageHost.GetSettingValue("TpsPrep")),0);
            }
            catch
            {
                prep_time = new TimeSpan(0, 45, 0);
                PackageHost.WriteWarn("Paramètre du temps de préparation non récupéré, le temps de 45mn sera considéré");
            }
            Alarme NewAlarm = new Alarme();
            NewAlarm.IsRinging = false;
            NewAlarm.TimeFirstEvent = WakeUp.Time.ToString();

            if (WakeUp.Time.Subtract(DateTime.Now) < new TimeSpan(1, 0, 0, 0)) //Si le réveil doit sonner dans + de 24h on active pas encore l'alarme et on attends le refresh journalier
            {
                if (WakeUp.TimeTraffic != new TimeSpan())
                {
                    WakeUp.Time = WakeUp.Time.Subtract(prep_time + WakeUp.TimeTraffic);
                    NewAlarm.TimeTraffic = WakeUp.TimeTraffic.ToString();
                    NewAlarm.AlarmHour = WakeUp.Time.Hour;
                    NewAlarm.AlarmMinutes = WakeUp.Time.Minute;
                    title = "Alarme en mode automatique";
                    body = string.Format("L'alarme sonnera à {0}:{1}", NewAlarm.AlarmHour.ToString("00"), NewAlarm.AlarmMinutes.ToString("00"));
                    PackageHost.SendMessage(MessageScope.Create("PushBullet"), "PushNote", new object[] { title as System.String, body as System.String });
                    ChangeAlarm(NewAlarm);
                    return;
                }
                else
                {
                    title = "Erreur temps de trajet";
                    body = "Impossible de récuperer le temps de trajet, le temps de trajet ne sera pas pris en compte";
                    PackageHost.SendMessage(MessageScope.Create("PushBullet"), "PushNote", new object[] { title as System.String, body as System.String });
                    WakeUp.Time = WakeUp.Time.Subtract(prep_time);
                    NewAlarm.AlarmHour = WakeUp.Time.Hour;
                    NewAlarm.AlarmMinutes = WakeUp.Time.Minute;
                    NewAlarm.TimeTraffic = new TimeSpan().ToString();
                    ChangeAlarm(NewAlarm);
                    return;
                }
            }
            else
            {
                PackageHost.WriteInfo("Pas de réveil automatique prévu d'ici les 24h prochaines heures !");
            }

        }

    }
}