using Constellation;
using Constellation.Package;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text;

namespace GeocodingApi
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
          

            //Boucle d'actualisation
            while (PackageHost.IsRunning)
            {
                
            }

        }

       

    }
}