using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Media;

namespace ReadSpeaker
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
}
