using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace Brain
{
    public partial class Program : PackageBase
    {
        public CancellationTokenSource StopWait = new CancellationTokenSource();
        public CancellationToken token { get; set; }
        private void ParametresServeur_ValueChanged(object sender, StateObjectChangedEventArgs e)
        {
            HaveToWait = false;
            StopWait.Cancel();
            PackageHost.WriteInfo("Changement de param");
            if (bool.Parse(e.NewState.DynamicValue.IsActive.ToString()))
            {
                PackageHost.WriteInfo("Le réveil est maintenant actif");
                if(bool.Parse(e.NewState.DynamicValue.ManualMode.ToString()))//Si en mode manuel
                {
                    PackageHost.WriteInfo("Configuration changée : Réveil en mode manuel ");
                    ModeManuel();
                }

                else //Si en mode automatique calendar
                {
                    PackageHost.WriteInfo("Configuration changée : Réveil en mode auto ");
                    ModeAuto();
                }
                     
            }
            else
            {
                PackageHost.WriteInfo("Le réveil n'est plus actif :(");
            }
        }

        private void Alarm_ValueChanged(object sender, StateObjectChangedEventArgs e)
        {
            HaveToWait = false;
            StopWait.Cancel();
            if (e.NewState.DynamicValue.AlarmHour >= 0 && !(bool.Parse(e.NewState.DynamicValue.IsRinging.ToString())))
            {
                HaveToWait = true;
                PackageHost.WriteInfo("L'alarme sonnera à {0}:{1}", e.NewState.DynamicValue.AlarmHour.ToString("00"), e.NewState.DynamicValue.AlarmMinutes.ToString("00"));
                PackageHost.WriteInfo("Goto wait()");
                Wait();                
            }

        }

        private void Events_ValueChanged(object sender, StateObjectChangedEventArgs e)
        {
            foreach(var event_ in e.NewState.DynamicValue)
            {
                //PackageHost.WriteInfo("{0} // {1}", DateTime.Parse(event_.DateDebut.ToString()) == DateTime.Parse(Alarm.DynamicValue.TimeFirstEvent.ToString()), TimeSpan.Parse(event_.TimeTrafficFromHouse.ToString()) != TimeSpan.Parse(Alarm.DynamicValue.TimeTraffic.ToString()));
                if(DateTime.Parse(event_.DateDebut.ToString()) > DateTime.Now && TimeSpan.Parse(event_.TimeTrafficFromHouse.ToString()) != TimeSpan.Parse(Alarm.DynamicValue.TimeTraffic.ToString())) 
                {
                   // PackageHost.WriteInfo("{0},{1}", TimeSpan.Parse(event_.TimeTrafficFromHouse.ToString()), TimeSpan.Parse(Alarm.Value.DynamicValue.TimeTraffic.ToString()));
                    TimeSpan diff_traffic = TimeSpan.Parse(event_.TimeTrafficFromHouse.ToString()) - TimeSpan.Parse(Alarm.Value.DynamicValue.TimeTraffic.ToString()); //Nouveau temps - ancien temps
                    PackageHost.WriteInfo("Diff traffic {0}",diff_traffic);
                    TimeSpan BeforeAlarm = (DateTime.Parse(Alarm.DynamicValue.TimeFirstEvent.ToString()) - new TimeSpan(0, int.Parse(PackageHost.GetSettingValue("TpsPrep")), 0) - TimeSpan.Parse(event_.TimeTrafficFromHouse.ToString()) ).Subtract(DateTime.Now) ;
                    PackageHost.WriteDebug("Before alarm {0}", BeforeAlarm);
                    if (BeforeAlarm>diff_traffic)//Si le temps de trafic augmente on réveille plus tôt sans dépasser l'heure 
                    {
                        Alarme newAlarm = new Alarme();
                        newAlarm.AlarmMinutes = int.Parse(Alarm.Value.DynamicValue.AlarmMinutes.ToString()) - diff_traffic.Minutes;
                        PackageHost.WriteDebug("AlarmMinutes {0}", newAlarm.AlarmMinutes);
                        if (newAlarm.AlarmMinutes > 60)
                        {
                            newAlarm.AlarmHour = (int.Parse(Alarm.Value.DynamicValue.AlarmHour.ToString()) - diff_traffic.Hours + newAlarm.AlarmMinutes/60)%24;
                            newAlarm.AlarmMinutes = newAlarm.AlarmMinutes%60;
                        }
                        if(newAlarm.AlarmMinutes<0)
                        {
                            newAlarm.AlarmHour = (int.Parse(Alarm.Value.DynamicValue.AlarmHour.ToString()) - diff_traffic.Hours + (newAlarm.AlarmMinutes-60 / 60))%24;
                            newAlarm.AlarmMinutes = newAlarm.AlarmMinutes % 60;

                        }
                        else
                        {
                            newAlarm.AlarmHour = (int.Parse(Alarm.Value.DynamicValue.AlarmHour.ToString()) - diff_traffic.Hours)%24;
                        }

                        newAlarm.IsRinging = false;
                        PackageHost.WriteDebug("Is ringing : {0}", newAlarm.IsRinging);
                        newAlarm.TimeFirstEvent = event_.DateDebut.ToString();
                        PackageHost.WriteDebug("Time 1st event {0}", newAlarm.TimeFirstEvent);
                        newAlarm.TimeTraffic = event_.TimeTrafficFromHouse.ToString();
                        PackageHost.WriteDebug("New time traffic {0}", newAlarm.TimeTraffic);
                        ChangeAlarm(newAlarm);
                        return;
                    }
                    return;
                }
            }
        }


    }



}