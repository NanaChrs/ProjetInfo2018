using Constellation.Package;
using Newtonsoft.Json;
using System;
using System.Net;

namespace GeocodingApi
{

    public partial class Program : PackageBase
    {
        /// <summary>
        /// Permet de récupérer les coordonnées Latitude/Longitude d'une adresse
        /// </summary>
        /// <param name="adress"></param>
        [MessageCallback]
        public Coordonnes CoordoneesGPS(string adress)
        {
            var coord = new Coordonnes();
            string APIkey = "AIzaSyB_oKCbvTHtGuIg0vbRsnqD9JgIQNOWMno";
            adress = adress.Replace(" ", "+");
            var url = "https://maps.googleapis.com/maps/api/geocode/json?address=" + adress + "+&key=" + APIkey;
            using (WebClient webClient = new System.Net.WebClient())
            {
                WebClient n = new WebClient();
                var json = n.DownloadString(url);
                string valueOriginal = Convert.ToString(json);
                Geocoding.Rootobject GPS = JsonConvert.DeserializeObject<Geocoding.Rootobject>(json);
                double lat = GPS.results[0].geometry.location.lat;
                double lng = GPS.results[0].geometry.location.lng;
                coord.latitude = lat;
                coord.longitude = lng;

                if (GPS.results != null)
                {
                    try
                    {
                        PackageHost.PushStateObject("Coordonnees GPS", coord);
                        return coord;
                    }
                    catch
                    {
                        PackageHost.WriteError("Impossible de push le stateobject");
                        
                    }
                }
                else
                {
                    PackageHost.WriteWarn("Aucune correspondance GPS trouvée");
                }
                return null;
            }
            
            
        }

    }
}