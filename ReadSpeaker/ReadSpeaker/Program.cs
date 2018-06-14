using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Media;


namespace ReadSpeaker
{
    public class Program : PackageBase
    {

        /// <summary>
        /// Permet de lire un texte !
        /// </summary>
        /// <param name="Texte">Le texte à lire</param>
        /// <param name="speed">Donne la vitesse de lecture entre 0 et 200 (100 par défaut)</param>
        [MessageCallback]
        public static void LireTexte(string SCAPItext, int speed)
        {
            string SCAPIkey = "23e0400a820db66ea31c14ab53c8612f";
            string SCAPIlang = "fr_fr";
            string SCAPIvoice = "Roxane";
            var url = "https://tts.readspeaker.com/a/speak?key=" + SCAPIkey + "&lang=" + SCAPIlang + "&voice=" + SCAPIvoice + "&text=" + SCAPItext + "&container=wav&audioformat=pcm&streaming=0&speed=" + speed.ToString();
            //surround with try/catch block. I believe it's WebException that should be used "catch(WebException e)"
            using (MemoryStream ms = new MemoryStream())
            {
                var response = WebRequest.Create(url).GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    int size = (int)response.ContentLength;
                    byte[] buffer = new byte[size];
                    int read;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                }

                Console.WriteLine("Capacity = {0}, Length = {1}, Position = {2}\n",
                         ms.Capacity.ToString(),
                         ms.Length.ToString(),
                         ms.Position.ToString());

                // Play audio from MemoryStream.
                ms.Position = 0;
                MediaPlayer mp = new MediaPlayer(ms);
                mp.Play();


            }
        }

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
            LireTexte("Bonjour,Initialisation terminée, il est" + (DateTime.Now.Hour).ToString() + "heures " + (DateTime.Now.Minute).ToString(), 75);
        }
    }
}
