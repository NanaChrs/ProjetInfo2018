using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Media;

namespace audiofile
{

    class MediaPlayer
    {

        System.Media.SoundPlayer soundPlayer;
        bool run = true;

        //Play from MemoryStream
        public MediaPlayer(MemoryStream stream)
        {
            soundPlayer = new System.Media.SoundPlayer(stream);
            soundPlayer.LoadCompleted += new AsyncCompletedEventHandler(player_LoadCompleted);
            soundPlayer.Load();
        }

        //Play from file
        public MediaPlayer(String filePath)
        {
            soundPlayer = new System.Media.SoundPlayer(filePath);
            soundPlayer.LoadCompleted += new AsyncCompletedEventHandler(player_LoadCompleted);
            soundPlayer.Load();
        }

        public void Play()
        {
            soundPlayer.Play();
        }
        // Handler for the LoadCompleted event.
        private void player_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string message = String.Format("LoadCompleted");
            Console.WriteLine(message);
        }
    }

    class Program
    {

        public static void PlayWavFromUrl(string url)
        {
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

        static int Main(string[] args)
        {

            string SCAPIkey = "23e0400a820db66ea31c14ab53c8612f";
            string SCAPIlang = "fr_fr";
            string SCAPIvoice = "Roxane";
            string SCAPItext = "Bonjour, il est"+(DateTime.Now.Hour).ToString()+ "heures "+(DateTime.Now.Minute).ToString()+"";

            PlayWavFromUrl("https://tts.readspeaker.com/a/speak?key=" + SCAPIkey + "&lang=" + SCAPIlang + "&voice=" + SCAPIvoice + "&text=" + SCAPItext + "&container=wav&audioformat=pcm&streaming=0&speed=80");

            Console.WriteLine("You should now have heard the text beeing read to you.");
            Console.WriteLine("Press enter to exit the application");
            Console.ReadLine();

            return 0;
        }
    }
}
