﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TimeIt.Models;

namespace TimeIt.Migrations
{
    [DbContext(typeof(TimeItDbContext))]
    [Migration("20200417013202_Updated_Ef_Core")]
    partial class Updated_Ef_Core
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3");

            modelBuilder.Entity("TimeIt.Models.Interval", b =>
                {
                    b.Property<int>("IntervalID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Duration")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Position")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TimerID")
                        .HasColumnType("INTEGER");

                    b.HasKey("IntervalID");

                    b.HasIndex("TimerID");

                    b.HasIndex("TimerID", "Position")
                        .IsUnique();

                    b.ToTable("Intervals");

                    b.HasData(
                        new
                        {
                            IntervalID = 1,
                            Color = "#FFFFFF00",
                            Duration = 180,
                            Name = "Workout",
                            Position = 1,
                            TimerID = 1
                        },
                        new
                        {
                            IntervalID = 2,
                            Color = "#FF0000FF",
                            Duration = 40,
                            Name = "Rest",
                            Position = 2,
                            TimerID = 1
                        },
                        new
                        {
                            IntervalID = 3,
                            Color = "#FFFF0000",
                            Duration = 90,
                            Name = "Run",
                            Position = 3,
                            TimerID = 1
                        },
                        new
                        {
                            IntervalID = 4,
                            Color = "#FFFF0000",
                            Duration = 180,
                            Name = "Distrito Capital",
                            Position = 1,
                            TimerID = 2
                        },
                        new
                        {
                            IntervalID = 5,
                            Color = "#FFFFFFFF",
                            Duration = 340,
                            Name = "Zulia",
                            Position = 2,
                            TimerID = 2
                        },
                        new
                        {
                            IntervalID = 6,
                            Color = "#FFFFFF00",
                            Duration = 300,
                            Name = "Anzoategui",
                            Position = 3,
                            TimerID = 2
                        },
                        new
                        {
                            IntervalID = 7,
                            Color = "#FFEE82EE",
                            Duration = 240,
                            Name = "Nueva Esparta",
                            Position = 4,
                            TimerID = 2
                        },
                        new
                        {
                            IntervalID = 8,
                            Color = "#FFF5FFFA",
                            Duration = 290,
                            Name = "Guarico",
                            Position = 5,
                            TimerID = 2
                        },
                        new
                        {
                            IntervalID = 9,
                            Color = "#FFF0F8FF",
                            Duration = 280,
                            Name = "Trujillo",
                            Position = 6,
                            TimerID = 2
                        },
                        new
                        {
                            IntervalID = 10,
                            Color = "#FFCD5C5C",
                            Duration = 220,
                            Name = "Tachira",
                            Position = 7,
                            TimerID = 2
                        },
                        new
                        {
                            IntervalID = 11,
                            Color = "#FF008000",
                            Duration = 180,
                            Name = "Aragua",
                            Position = 8,
                            TimerID = 2
                        },
                        new
                        {
                            IntervalID = 12,
                            Color = "#FFADFF2F",
                            Duration = 150,
                            Name = "Bolivar",
                            Position = 9,
                            TimerID = 2
                        },
                        new
                        {
                            IntervalID = 13,
                            Color = "#FF7CFC00",
                            Duration = 345,
                            Name = "Aragua",
                            Position = 10,
                            TimerID = 2
                        },
                        new
                        {
                            IntervalID = 14,
                            Color = "#FFF0E68C",
                            Duration = 235,
                            Name = "Delta Amacuro",
                            Position = 11,
                            TimerID = 2
                        });
                });

            modelBuilder.Entity("TimeIt.Models.Timer", b =>
                {
                    b.Property<int>("TimerID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Repetitions")
                        .HasColumnType("INTEGER");

                    b.HasKey("TimerID");

                    b.ToTable("Timers");

                    b.HasData(
                        new
                        {
                            TimerID = 1,
                            Name = "Default",
                            Repetitions = 2
                        },
                        new
                        {
                            TimerID = 2,
                            Name = "Estados sin luz",
                            Repetitions = 2
                        });
                });

            modelBuilder.Entity("TimeIt.Models.Interval", b =>
                {
                    b.HasOne("TimeIt.Models.Timer", "Timer")
                        .WithMany("Intervals")
                        .HasForeignKey("TimerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}