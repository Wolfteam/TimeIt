﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TimeIt.Models;

namespace TimeIt.Migrations
{
    [DbContext(typeof(TimeItDbContext))]
    partial class TimeItDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028");

            modelBuilder.Entity("TimeIt.Models.Interval", b =>
                {
                    b.Property<int>("IntervalID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Color")
                        .IsRequired();

                    b.Property<int>("Duration");

                    b.Property<string>("Name");

                    b.Property<int>("Position");

                    b.Property<int>("TimerID");

                    b.HasKey("IntervalID");

                    b.HasIndex("TimerID");

                    b.ToTable("Intervals");
                });

            modelBuilder.Entity("TimeIt.Models.Timer", b =>
                {
                    b.Property<int>("TimerID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("Repetitions");

                    b.HasKey("TimerID");

                    b.ToTable("Timers");
                });

            modelBuilder.Entity("TimeIt.Models.Interval", b =>
                {
                    b.HasOne("TimeIt.Models.Timer", "Timer")
                        .WithMany("Intervals")
                        .HasForeignKey("TimerID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}