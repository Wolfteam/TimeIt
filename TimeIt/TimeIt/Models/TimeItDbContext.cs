using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using TimeIt.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TimeIt.Models
{
    public class TimeItDbContext : DbContext
    {
        private const string databaseName = "timeIt.db";
        private const string currentMigration = "Migration_v1";

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

            modelBuilder.Entity<Interval>()
                .HasIndex(t => t.TimerID);

            modelBuilder.Entity<Interval>()
                .HasIndex(t => new { t.TimerID, t.Position }).IsUnique();

            modelBuilder.Entity<Interval>()
                .HasOne(i => i.Timer)
                .WithMany(t => t.Intervals)
                .HasForeignKey(i => i.TimerID);

            modelBuilder.Entity<Timer>().HasData(
                new Timer
                {
                    TimerID = 1,
                    Name = "Default",
                    Repetitions = 2
                }, new Timer
                {
                    TimerID = 2,
                    Name = "Estados sin luz",
                    Repetitions = 2
                }
            );

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
                },
                new Interval
                {
                    IntervalID = 4,
                    Color = Color.Red,
                    Duration = 180,
                    Name = "Distrito Capital",
                    Position = 1,
                    TimerID = 2
                },
                new Interval
                {
                    IntervalID = 5,
                    Color = Color.White,
                    Duration = 340,
                    Name = "Zulia",
                    Position = 2,
                    TimerID = 2
                },
                new Interval
                {
                    IntervalID = 6,
                    Color = Color.Yellow,
                    Duration = 300,
                    Name = "Anzoategui",
                    Position = 3,
                    TimerID = 2
                },
                new Interval
                {
                    IntervalID = 7,
                    Color = Color.Violet,
                    Duration = 240,
                    Name = "Nueva Esparta",
                    Position = 4,
                    TimerID = 2
                },
                new Interval
                {
                    IntervalID = 8,
                    Color = Color.MintCream,
                    Duration = 290,
                    Name = "Guarico",
                    Position = 5,
                    TimerID = 2
                },
                new Interval
                {
                    IntervalID = 9,
                    Color = Color.AliceBlue,
                    Duration = 280,
                    Name = "Trujillo",
                    Position = 6,
                    TimerID = 2
                },
                new Interval
                {
                    IntervalID = 10,
                    Color = Color.IndianRed,
                    Duration = 220,
                    Name = "Tachira",
                    Position = 7,
                    TimerID = 2
                },
                new Interval
                {
                    IntervalID = 11,
                    Color = Color.Green,
                    Duration = 180,
                    Name = "Aragua",
                    Position = 8,
                    TimerID = 2
                },
                new Interval
                {
                    IntervalID = 12,
                    Color = Color.GreenYellow,
                    Duration = 150,
                    Name = "Bolivar",
                    Position = 9,
                    TimerID = 2
                },
                new Interval
                {
                    IntervalID = 13,
                    Color = Color.LawnGreen,
                    Duration = 345,
                    Name = "Aragua",
                    Position = 10,
                    TimerID = 2
                },
                new Interval
                {
                    IntervalID = 14,
                    Color = Color.Khaki,
                    Duration = 235,
                    Name = "Delta Amacuro",
                    Position = 11,
                    TimerID = 2
                }
           );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string databasePath = "";
            //if you want to run migrations, you need to comment this code...
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
                    databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), databaseName);
                    break;
                default:
                    throw new NotImplementedException("Platform not supported");
            }
            // Specify that we will use sqlite and the path of the database here
            optionsBuilder.UseSqlite($"Filename={databasePath}");
        }

        public static void Init()
        {
            bool migrated = Preferences.ContainsKey(currentMigration);
            if (migrated)
                return;
            using (var context = new TimeItDbContext())
            {
                context.Database.Migrate();
                Preferences.Set(currentMigration, true);
            }
        }
    }
}
