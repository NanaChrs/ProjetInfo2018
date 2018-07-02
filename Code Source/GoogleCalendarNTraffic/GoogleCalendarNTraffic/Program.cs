using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleCalendarNTraffic
{
    public partial class Program : PackageBase
    {
        [StateObjectLink("GoogleCalendar", "Events")]
        public StateObjectNotifier Events { get; set; }

        [StateObjectLink("GoogleCalendarNTraffic", "EventWithTraffic")]
        public StateObjectNotifier SO_EventWithTraffic { get; set; }


        public int Interval { get; set; }

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
            string Maison;
            try
            {
                Maison = PackageHost.GetSettingValue<string>("Adresse de la maison");
            }
            catch
            {
               Maison = "1 Vieux chemin des loups bailleul";
            }
            

            Task.Factory.StartNew(() =>
            {
                while (PackageHost.IsRunning)
                {
                    GetAgendaNTraffic(Maison);
                    try
                    {
                        Thread.Sleep(PackageHost.GetSettingValue<int>("Interval de rafraichissement")); 
                    }
                    catch
                    {
                        Thread.Sleep(300000);
                    }
                    
                }
            });


        }


    }
}
