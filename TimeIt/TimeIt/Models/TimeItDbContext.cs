using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using TimeIt.Extensions;
using Xamarin.Forms;

namespace TimeIt.Models
{
    public class TimeItDbContext : DbContext
    {
        private const string databaseName = "timeIt.db";

        public DbSet<Timer> Timers { get; set; }
        public DbSet<Interval> Intervals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Interval>()
                .Property(i => i.Color)
                .HasConversion(
                    v => v.ToHexString(true),
                    v => Color.FromHex(v));

            modelBuilder.Entity<Timer>().HasData(new Timer
            {
                TimerID = 1,
                Name = "Default",
                Repetitions = 2
            });

            modelBuilder.Entity<Interval>().HasData(
                new Interval
                {
                    IntervalID = 1,
                    Color = Color.Yellow,
                    Duration = 180,
                    Name = "Workout",
                    Position = 1,
                    TimerID = 1
                },
                new Interval
                {
                    IntervalID = 2,
                    Color = Color.Blue,
                    Duration = 40,
                    Name = "Rest",
                    Position = 2,
                    TimerID = 1
                },
                new Interval
                {
                    IntervalID = 3,
                    Color = Color.Red,
                    Duration = 90,
                    Name = "Run",
                    Position = 3,
                    TimerID = 1
                }
           );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string databasePath = "";
            //if you want to run migrations, you need to move to comment this code...
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    SQLitePCL.Batteries_V2.Init();
                    databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", databaseName); ;
                    break;
                case Device.Android:
                    databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), databaseName);
                    break;
                case Device.UWP:
                    databasePath = databaseName;
                    break;
                default:
                    throw new NotImplementedException("Platform not supported");
            }
            // Specify that we will use sqlite and the path of the database here
            optionsBuilder.UseSqlite($"Filename={databasePath}");
        }

        public static void Init()
        {
            using (var context = new TimeItDbContext())
            {
                context.Database.Migrate();
            }
        }
    }
}
