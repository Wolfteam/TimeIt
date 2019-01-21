using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
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
                    v => v.ToString(), 
                    v => Color.FromHex(v));
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
