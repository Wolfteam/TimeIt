using Plugin.SimpleAudioPlayer;
using System;
using System.IO;

namespace TimeIt.Helpers
{
    public class SoundHelper
    {
        public static void PlaySound(string filename, double volume = 100)
        {
            try
            {
                var player = CrossSimpleAudioPlayer.Current;
                player.Volume = volume;
                player.Load(filename);
                player.Play();
            }
            catch (Exception)
            {
                //well
            }
        }

        public static void PlaySound(Stream stream, double volume = 100)
        {
            try
            {
                var player = CrossSimpleAudioPlayer.Current;
                player.Volume = volume;
                player.Load(stream);
                player.Play();
            }
            catch (Exception)
            {
                //well
            }
        }
    }
}
