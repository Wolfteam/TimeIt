using System;

namespace TimeIt.Droid.Models
{
    public class LocalNotification
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public int Id { get; set; }
        public int IconId { get; set; }
        public string SoundPath { get; set; }
        //public DateTime NotifyTime { get; set; }
    }
}