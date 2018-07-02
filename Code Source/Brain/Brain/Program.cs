using Constellation;
using Constellation.Package;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace Brain
{
    public partial class Program : PackageBase
    {
        //public StateObjectNotifier ParametresServeur { get; set; }
        //public StateObjectNotifier Alarm { get; set; }

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            if (!ParametresServeur.HasValue)
            {
                PackageHost.WriteWarn("Configuration du réveil non trouvée, les valeurs par défaut sont utilisées");
                ParametresServeurParDefaut();
                System.Threading.Thread.Sleep(1000);
            }
            if (!Alarm.HasValue)
            {
                PackageHost.WriteWarn("Configuration de l'alarme non trouvée, les valeurs par défaut sont utilisées");
                AlarmParDefaut();
                System.Threading.Thread.Sleep(1000);
            }

            if (bool.Parse(ParametresServeur.DynamicValue.IsActive.ToString()))
            {
                if (!bool.Parse(ParametresServeur.DynamicValue.ManualMode.ToString()))
                {
                    PackageHost.WriteInfo("Mode automatique réactivé");
                    ModeAuto();
                }
                if (bool.Parse(ParametresServeur.DynamicValue.ManualMode.ToString()) && int.Parse(ParametresServeur.DynamicValue.AlarmHour.ToString()) > 0)
                {
                    PackageHost.WriteInfo("Mode manuel réactivé");
                    ModeManuel();
                }
                else
                {
                    PackageHost.WriteInfo("Pas d'alarme de programmé");
                }
            }
            //Après avoir récupéré des paramètres, on s'abonne au changements du SO
            this.ParametresServeur.ValueChanged += ParametresServeur_ValueChanged;
            this.Alarm.ValueChanged += Alarm_ValueChanged;
            this.Events.ValueChanged += Events_ValueChanged;
            //Initialisation à partir des paramètres...
            //Sinon attente


     
                Task.Factory.StartNew(async () =>
                {
                    while (PackageHost.IsRunning)
                    {
               
                    try
                    {
                            if (PackageHost.GetSettingValue<Int32>("HeureMaJ") == DateTime.Now.Hour && DateTime.Now.Minute == 0 && bool.Parse(ParametresServeur.DynamicValue.IsActive.ToString()))
                            {
                                if(bool.Parse(ParametresServeur.DynamicValue.ManualMode.ToString()))
                                {
                                    ModeManuel();
                                }
                                else
                                {
                                    ModeAuto();
                                }
                            }

                    }
                    catch (Exception e)
                    {
                       
                        PackageHost.WriteError("Impossible de mettre l'alarme à jour ! {0}",e);
                    }
                    await Task.Delay(new TimeSpan(0,1,0));
                }
                });
            
        }
    }
}     
 







