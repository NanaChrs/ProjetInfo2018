using Constellation.Package;
using System;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using System.Threading.Tasks;

namespace GoogleCalendar
{
    public partial class Program : PackageBase
    {

        // Create Google Calendar API service.
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }



#pragma warning disable CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement
        public override void OnStart()
#pragma warning restore CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement

        {
            if (!PackageHost.IsConnected)
            {
                Console.Write("Tentative de connection échouée");
            }
            //MaJ au démarrage
            GetAgenda();

            //Boucle d'actualisation
            while (PackageHost.IsRunning)
            {
                System.Threading.Thread.Sleep(PackageHost.GetSettingValue<Int32>("Interval"));
                PackageHost.WriteInfo("Interval de {0} ms passé, actualisation de l'agenda", PackageHost.GetSettingValue<Int32>("Interval"));
                GetAgenda();
            }              

        }

    }
}