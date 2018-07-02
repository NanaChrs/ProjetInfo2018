using Constellation.Package;
using System;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleCalendarNTraffic
{
    public partial class Program : PackageBase
    {
        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "Google Calendar API .NET Quickstart";

        /// <summary>
        /// Permet de mettre à jour le StateObject Events de Google CalendarNTraffic
        /// </summary>
        [MessageCallback]
        public List<EventnTraffic> GetAgendaNTraffic(string Maison)
        {
            var DonnesAgenda = new List<EventnTraffic>();
            UserCredential credential;
            using (
                var stream =
            new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine("calendar-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            try
            {
                request.MaxResults = int.Parse(PackageHost.GetSettingValue("Nombre d'evenements"));
            }
            catch
            {
                request.MaxResults = 5;
            }
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            Events events = request.Execute();
            if (events.Items != null && events.Items.Count > 0)
            {

                foreach (var eventItem in events.Items)
                {
                    string when = eventItem.Start.DateTime.ToString();
                    if (String.IsNullOrEmpty(when))
                    {
                        when = eventItem.Start.Date;
                    }
                    EventnTraffic a = new EventnTraffic();
                    a.DateDebut = eventItem.Start.DateTime.ToString();
                    a.DateFin = eventItem.End.DateTime.ToString();
                    a.Nom = eventItem.Summary;
                    a.Lieu = eventItem.Location;
                    //PackageHost.WriteInfo("{0} à {1}, {2}", a.Nom, a.DateDebut, a.Lieu);
                    if (!string.IsNullOrEmpty(a.Lieu) )
                    {
                        TimeSpan duree_trajet = new TimeSpan();
                        Task<dynamic> reponse = PackageHost.CreateMessageProxy("GoogleTraffic").GetRoutes<dynamic>(Maison, a.Lieu);
                        if (reponse.Wait(30000) && reponse.IsCompleted)
                        {
                            try
                            {
                                duree_trajet = TimeSpan.Parse(reponse.Result[0].TimeWithTraffic.ToString());
                                //PackageHost.WriteInfo("Durée de trajet : {0}", duree_trajet.ToString());
                                a.TimeTrafficFromHouse = duree_trajet.ToString();
                            }

                            catch
                            {
                                PackageHost.WriteError("Impossible de trouver de temps de trajet pour {0}", a.Lieu);
                                a.TimeTrafficFromHouse = new TimeSpan().ToString();
                            }

                        }
                        else
                        {
                            PackageHost.WriteError("Aucune réponse !");
                        }
                    }
                    DonnesAgenda.Add(a);
                    //Console.WriteLine("{0} à {1}", a.Nom, a.DateDebut);
                }

            }
            else
            {
                Console.WriteLine("No upcoming events found.");
                PackageHost.WriteWarn("Aucun Evenement Trouvé !");
            }
            PackageHost.PushStateObject("EventWithTraffic", DonnesAgenda);

            return (DonnesAgenda);
        }


    }
}


