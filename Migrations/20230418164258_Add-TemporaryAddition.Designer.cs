﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ScheduleBot.DB;

#nullable disable

namespace ScheduleBot.Migrations
{
    [DbContext(typeof(ScheduleDbContext))]
    [Migration("20230418164258_Add-TemporaryAddition")]
    partial class AddTemporaryAddition
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ScheduleBot.DB.Entity.CompletedDiscipline", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ID"));

                    b.Property<byte>("Class")
                        .HasColumnType("smallint");

                    b.Property<string>("Lecturer")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Subgroup")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("Class");

                    b.ToTable("CompletedDisciplines");
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.Discipline", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ID"));

                    b.Property<byte>("Class")
                        .HasColumnType("smallint");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<TimeOnly>("EndTime")
                        .HasColumnType("time without time zone");

                    b.Property<bool>("IsCastom")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("boolean");

                    b.Property<string>("LectureHall")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Lecturer")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<TimeOnly>("StartTime")
                        .HasColumnType("time without time zone");

                    b.Property<string>("Subgroup")
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("Class");

                    b.ToTable("Disciplines");
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.ModeDTO", b =>
                {
                    b.Property<byte>("ID")
                        .HasColumnType("smallint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.ToTable("Modes");

                    b.HasData(
                        new
                        {
                            ID = (byte)0,
                            Name = "Default"
                        },
                        new
                        {
                            ID = (byte)1,
                            Name = "AddingDiscipline"
                        });
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.Progress", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ID"));

                    b.Property<string>("Discipline")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("Mark")
                        .HasColumnType("integer");

                    b.Property<string>("MarkTitle")
                        .HasColumnType("text");

                    b.Property<int>("Term")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("Progresses");
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.TelegramUser", b =>
                {
                    b.Property<long>("ChatID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ChatID"));

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<byte>("Mode")
                        .HasColumnType("smallint");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("ChatID");

                    b.HasIndex("Mode");

                    b.ToTable("TelegramUsers");
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.TemporaryAddition", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ID"));

                    b.Property<DateTime>("AddDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Counter")
                        .HasColumnType("integer");

                    b.Property<DateOnly?>("Date")
                        .HasColumnType("date");

                    b.Property<TimeOnly?>("EndTime")
                        .HasColumnType("time without time zone");

                    b.Property<string>("LectureHall")
                        .HasColumnType("text");

                    b.Property<string>("Lecturer")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<TimeOnly?>("StartTime")
                        .HasColumnType("time without time zone");

                    b.Property<long>("User")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("User");

                    b.ToTable("TemporaryAddition");
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.TypeDTO", b =>
                {
                    b.Property<byte>("ID")
                        .HasColumnType("smallint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.ToTable("Types");

                    b.HasData(
                        new
                        {
                            ID = (byte)0,
                            Name = "all"
                        },
                        new
                        {
                            ID = (byte)1,
                            Name = "lab"
                        },
                        new
                        {
                            ID = (byte)2,
                            Name = "practice"
                        },
                        new
                        {
                            ID = (byte)3,
                            Name = "lecture"
                        },
                        new
                        {
                            ID = (byte)4,
                            Name = "other"
                        });
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.CompletedDiscipline", b =>
                {
                    b.HasOne("ScheduleBot.DB.Entity.TypeDTO", "TypeDTO")
                        .WithMany()
                        .HasForeignKey("Class")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TypeDTO");
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.Discipline", b =>
                {
                    b.HasOne("ScheduleBot.DB.Entity.TypeDTO", "TypeDTO")
                        .WithMany()
                        .HasForeignKey("Class")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TypeDTO");
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.TelegramUser", b =>
                {
                    b.HasOne("ScheduleBot.DB.Entity.ModeDTO", "ModeDTO")
                        .WithMany()
                        .HasForeignKey("Mode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ModeDTO");
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.TemporaryAddition", b =>
                {
                    b.HasOne("ScheduleBot.DB.Entity.TelegramUser", "TelegramUser")
                        .WithMany()
                        .HasForeignKey("User")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TelegramUser");
                });
#pragma warning restore 612, 618
        }
    }
}
