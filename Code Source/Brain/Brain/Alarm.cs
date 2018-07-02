using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Constellation.Package;
using System.Threading;

namespace Brain
{
    public partial class Program : PackageBase
    {
        public bool HaveToWait { get; set; }
        private void Ring()
        {
            PackageHost.WriteInfo("Bip Bip Bip");
            var NextAlarm = new Alarme();
            NextAlarm.IsRinging = true;
            NextAlarm.AlarmHour = DateTime.Now.Hour;
            NextAlarm.AlarmMinutes = DateTime.Now.Minute;
            NextAlarm.TimeFirstEvent = Alarm.DynamicValue.TimeFirstEvent.ToString();
            NextAlarm.TimeTraffic = Alarm.DynamicValue.TimeTraffic.ToString();
            ChangeAlarm(NextAlarm);
            return;
        }
        public void Wait()
        {
            TimeSpan WaitTime = new TimeSpan();
           
            if(!bool.Parse(ParametresServeur.DynamicValue.ManualMode.ToString()))
            {
                DateTime DateEvent = DateTime.Parse(Alarm.DynamicValue.TimeFirstEvent.ToString());
                PackageHost.WriteInfo("Heure Event {0}", DateEvent);
                var TempsPrep = new TimeSpan(0, int.Parse(PackageHost.GetSettingValue("TpsPrep")), 0);
                PackageHost.WriteInfo("Moins prep {0}", TempsPrep);
                var TimeTraffic = TimeSpan.Parse(Alarm.DynamicValue.TimeTraffic.ToString());
                PackageHost.WriteInfo("Moins Traffic{0}", TimeTraffic);
                var HeureAlarme = DateEvent.Subtract(TempsPrep.Add(TimeTraffic));
                PackageHost.WriteInfo("Heure de réveil : {0}", HeureAlarme);
                WaitTime = HeureAlarme.Subtract(DateTime.Now);
            }
            if (bool.Parse(ParametresServeur.DynamicValue.ManualMode.ToString()))
            {
                WaitTime = new TimeSpan((int.Parse(Alarm.DynamicValue.AlarmHour.ToString()) - DateTime.Now.Hour - (int.Parse(Alarm.DynamicValue.AlarmMinutes.ToString()) - DateTime.Now.Minute ) / 60) % 24, (int.Parse(Alarm.DynamicValue.AlarmMinutes.ToString())-DateTime.Now.Minute-1)%60,60-DateTime.Now.Second);
            }
            PackageHost.WriteInfo("Temps d'attente {0}", WaitTime);
            StopWait = new CancellationTokenSource();
            token = StopWait.Token;
                    Task.Factory.StartNew(async () =>
                    {
                        while (HaveToWait)
                        {
                            await Task.Delay(WaitTime,token);
                            Ring();
                            token.ThrowIfCancellationRequested();
                            return;
                        }
                        PackageHost.WriteWarn("Arrêt de l'attente inattendue !");
                        return;
                    },token);

        }
    }
}
