using Plugin.SimpleAudioPlayer;
using System;
using System.IO;

namespace TimeIt.Helpers
{
    public class SoundHelper
    {
        public static void PlaySound(string filename, double volume = 100, Action onComplete = null)
        {
            try
            {
                var player = CrossSimpleAudioPlayer.Current;
                player.Volume = volume;
                if (onComplete != null)
                    player.PlaybackEnded += (sender, args) => onComplete();
                player.Load(filename);
                player.Play();
            }
            catch (Exception e)
            {
                throw e;
                //well
            }
        }

        public static void PlaySound(Stream stream, double volume = 100, Action onComplete = null)
        {
            try
            {
                var player = CrossSimpleAudioPlayer.Current;
                player.Volume = volume;
                if (onComplete != null)
                    player.PlaybackEnded += (sender, args) => onComplete();
                player.Load(stream);
                player.Play();
            }
            catch (Exception e)
            {
                throw e;
                //well
            }
        }
    }
}