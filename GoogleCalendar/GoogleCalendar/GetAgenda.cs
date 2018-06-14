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



namespace GoogleCalendar
{
    public partial class Program : PackageBase
    {
        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "Google Calendar API .NET Quickstart";

        /// <summary>
        /// Permet de mettre à jour le StateObject Events de Google Calendar
        /// </summary>
        [MessageCallback]
        static List<Event> GetAgenda()
        {
            var DonnesAgenda = new List<Event>();
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
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            Events events = request.Execute();
            Console.WriteLine("Upcoming events:");
            if (events.Items != null && events.Items.Count > 0)
            {
                
                foreach (var eventItem in events.Items)
                {
                    string when = eventItem.Start.DateTime.ToString();
                    if (String.IsNullOrEmpty(when))
                    {
                        when = eventItem.Start.Date;
                    }
                    Event a = new Event();
                    a.DateDebut = eventItem.Start.DateTime.ToString();
                    a.DateFin = eventItem.End.DateTime.ToString();
                    a.Nom = eventItem.Summary;
                    a.Lieu = eventItem.Location;
                    DonnesAgenda.Add(a);
                    Console.WriteLine("{0} à {1}", a.Nom, a.DateDebut);
                    PackageHost.WriteInfo("{0} à {1}", a.Nom, a.DateDebut);
                }

            }
            else
            {
                Console.WriteLine("No upcoming events found.");
                PackageHost.WriteWarn("Aucun Evenement Trouvé !");
            }
        try {
            PackageHost.PushStateObject("Events", DonnesAgenda);
            }
        catch
            {
                PackageHost.WriteError("Impossible de push le stateobject");
            }
        
        return (DonnesAgenda);   
        }
       
    }
}
    

