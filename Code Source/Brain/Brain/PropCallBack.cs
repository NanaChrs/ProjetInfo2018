using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Brain
{
    public partial class Program : PackageBase
    {
        [StateObjectLink("*", "Parametres_reveil")]
        public StateObjectNotifier ParametresServeur { get; set; }

        [StateObjectLink("*", "Alarm")]
        public StateObjectNotifier Alarm { get; set; }

        [StateObjectLink("GoogleCalendarNTraffic", "EventWithTraffic")]
        public StateObjectNotifier Events { get; set; }

        [StateObjectLink("*", "etat_reveil")]
        public StateObjectNotifier Etat_Reveil { get; set; }

        /// <summary>
        /// Permet de régler les paramètres de l'alarme
        /// </summary>
        /// <param name="newParametres">de type ParametresReveil { bool IsActive, bool BigSleeper, bool ManualMode</param>
        [MessageCallback]
        public void ChangeParametresServeur(ParametresReveil newParametres)
        {
            PackageHost.PushStateObject("Parametres_reveil", newParametres);
        }

        public void ChangeAlarm(Alarme newAlarm)
        {
            PackageHost.PushStateObject("Alarm", newAlarm);
            System.Threading.Thread.Sleep(1000);
        }

        /// <summary>
        /// Fonction qui permet d'éteindre l'alarme 
        /// </summary>
        [MessageCallback]
        public void StopAlarm()
        {
            AlarmParDefaut();
        }
        /// <summary>
        /// Permet de mettre l'alarme plus tard selon les paramètres précédement entrés
        /// </summary>
        [MessageCallback]
        public void SnoozeAlarm()
        {
             Alarme newAlarm = new Alarme();
            int SnoozeMinutes;
            try
            {
                if(bool.Parse(ParametresServeur.DynamicValue.BigSleeper.ToString()))
                {
                    SnoozeMinutes = int.Parse(PackageHost.GetSettingValue("Snooze").ToString());
                }
                else
                {
                    SnoozeMinutes = int.Parse(PackageHost.GetSettingValue("SnoozeBigSleeper").ToString());
                }
                
            }
            catch (Exception e)
            {
                PackageHost.WriteWarn(e);
                SnoozeMinutes=5;
            }

             if (int.Parse(Alarm.Value.DynamicValue.AlarmMinutes.ToString()) >= 55)
             {
                newAlarm.AlarmHour = int.Parse(Alarm.Value.DynamicValue.AlarmHour.ToString()) + 1;
             }
             else
             {
                 newAlarm.AlarmHour = int.Parse(Alarm.Value.DynamicValue.AlarmHour.ToString());
             }
             newAlarm.AlarmMinutes = (int.Parse(Alarm.Value.DynamicValue.AlarmMinutes.ToString()) + 5) % 60;
             newAlarm.IsRinging = false;
             newAlarm.TimeFirstEvent = Alarm.Value.DynamicValue.TimeFirstEvent;
             newAlarm.TimeTraffic = Alarm.Value.DynamicValue.TimeTraffic;
             ChangeAlarm(newAlarm);
            
        }
    

        public void ParametresServeurParDefaut()
        {
            var ParamDef = new ParametresReveil();
            ParamDef.IsActive = false;
            ParamDef.ManualMode = false;
            ParamDef.BigSleeper = false;
            ParamDef.ManualAlarmHour = -1;
            ParamDef.ManualAlarmMinute = -1;
            ChangeParametresServeur(ParamDef);
        }

        public void AlarmParDefaut()
        {
            var ParamDef = new Alarme();
            ParamDef.IsRinging = false;
            ParamDef.AlarmHour = -1;
            ParamDef.AlarmMinutes = -1;
            ChangeAlarm(ParamDef);
        }




    }
}








