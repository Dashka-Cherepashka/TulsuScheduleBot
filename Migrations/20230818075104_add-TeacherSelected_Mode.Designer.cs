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
    [Migration("20230818075104_add-TeacherSelected_Mode")]
    partial class addTeacherSelected_Mode
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ScheduleBot.DB.Entity.ClassDTO", b =>
                {
                    b.Property<byte>("ID")
                        .HasColumnType("smallint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.ToTable("Classes");

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
                        },
                        new
                        {
                            ID = (byte)5,
                            Name = "custom"
                        });
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.CompletedDiscipline", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ID"));

                    b.Property<byte>("Class")
                        .HasColumnType("smallint");

                    b.Property<DateOnly?>("Date")
                        .HasColumnType("date");

                    b.Property<string>("Lecturer")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("ScheduleProfileGuid")
                        .HasColumnType("uuid");

                    b.Property<string>("Subgroup")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("Class");

                    b.HasIndex("ScheduleProfileGuid");

                    b.ToTable("CompletedDisciplines");
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.CustomDiscipline", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ID"));

                    b.Property<DateTime>("AddDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Counter")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<TimeOnly?>("EndTime")
                        .HasColumnType("time without time zone");

                    b.Property<bool>("IsAdded")
                        .HasColumnType("boolean");

                    b.Property<string>("LectureHall")
                        .HasColumnType("text");

                    b.Property<string>("Lecturer")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid>("ScheduleProfileGuid")
                        .HasColumnType("uuid");

                    b.Property<TimeOnly?>("StartTime")
                        .HasColumnType("time without time zone");

                    b.Property<string>("Type")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("ScheduleProfileGuid");

                    b.ToTable("CustomDiscipline");
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

                    b.Property<string>("Group")
                        .IsRequired()
                        .HasColumnType("text");

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

            modelBuilder.Entity("ScheduleBot.DB.Entity.GroupLastUpdate", b =>
                {
                    b.Property<string>("Group")
                        .HasColumnType("text");

                    b.Property<DateTime>("Update")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Group");

                    b.ToTable("GroupLastUpdate");
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.MessageLog", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ID"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("TelegramUserChatID")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("TelegramUserChatID");

                    b.ToTable("MessageLog");
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
                        },
                        new
                        {
                            ID = (byte)2,
                            Name = "GroupСhange"
                        },
                        new
                        {
                            ID = (byte)3,
                            Name = "StudentIDСhange"
                        },
                        new
                        {
                            ID = (byte)4,
                            Name = "ResetProfileLink"
                        },
                        new
                        {
                            ID = (byte)5,
                            Name = "CustomEditName"
                        },
                        new
                        {
                            ID = (byte)6,
                            Name = "CustomEditLecturer"
                        },
                        new
                        {
                            ID = (byte)7,
                            Name = "CustomEditType"
                        },
                        new
                        {
                            ID = (byte)8,
                            Name = "CustomEditLectureHall"
                        },
                        new
                        {
                            ID = (byte)9,
                            Name = "CustomEditStartTime"
                        },
                        new
                        {
                            ID = (byte)10,
                            Name = "CustomEditEndTime"
                        },
                        new
                        {
                            ID = (byte)11,
                            Name = "DaysNotifications"
                        },
                        new
                        {
                            ID = (byte)12,
                            Name = "TeachersWorkSchedule"
                        },
                        new
                        {
                            ID = (byte)13,
                            Name = "TeacherSelected"
                        });
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.Notifications", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ID"));

                    b.Property<int>("Days")
                        .HasColumnType("integer");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("boolean");

                    b.Property<long?>("OwnerID")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("OwnerID")
                        .IsUnique();

                    b.ToTable("Notifications");
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

                    b.Property<string>("StudentID")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Term")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("Progresses");
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.ScheduleProfile", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Group")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastAppeal")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long?>("OwnerID")
                        .HasColumnType("bigint");

                    b.Property<string>("StudentID")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("Group");

                    b.HasIndex("OwnerID");

                    b.HasIndex("StudentID");

                    b.ToTable("ScheduleProfile");
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.StudentIDLastUpdate", b =>
                {
                    b.Property<string>("StudentID")
                        .HasColumnType("text");

                    b.Property<DateTime>("Update")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("StudentID");

                    b.ToTable("StudentIDLastUpdate");
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.TeacherLastUpdate", b =>
                {
                    b.Property<string>("Teacher")
                        .HasColumnType("text");

                    b.Property<DateTime>("Update")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Teacher");

                    b.ToTable("TeacherLastUpdate");
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.TeacherWorkSchedule", b =>
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

                    b.Property<string>("Groups")
                        .HasColumnType("text");

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

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("Class");

                    b.HasIndex("Lecturer");

                    b.ToTable("TeacherWorkSchedule");
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

                    b.Property<DateTime>("LastAppeal")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<byte>("Mode")
                        .HasColumnType("smallint");

                    b.Property<long>("NotificationsID")
                        .HasColumnType("bigint");

                    b.Property<int?>("RequestingMessageID")
                        .HasColumnType("integer");

                    b.Property<Guid>("ScheduleProfileGuid")
                        .HasColumnType("uuid");

                    b.Property<string>("TempData")
                        .HasColumnType("text");

                    b.Property<long>("TodayRequests")
                        .HasColumnType("bigint");

                    b.Property<long>("TotalRequests")
                        .HasColumnType("bigint");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("ChatID");

                    b.HasIndex("Mode");

                    b.HasIndex("NotificationsID");

                    b.HasIndex("ScheduleProfileGuid");

                    b.ToTable("TelegramUsers");
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.CompletedDiscipline", b =>
                {
                    b.HasOne("ScheduleBot.DB.Entity.ClassDTO", "ClassDTO")
                        .WithMany()
                        .HasForeignKey("Class")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ScheduleBot.DB.Entity.ScheduleProfile", "ScheduleProfile")
                        .WithMany()
                        .HasForeignKey("ScheduleProfileGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ClassDTO");

                    b.Navigation("ScheduleProfile");
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.CustomDiscipline", b =>
                {
                    b.HasOne("ScheduleBot.DB.Entity.ScheduleProfile", "ScheduleProfile")
                        .WithMany()
                        .HasForeignKey("ScheduleProfileGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ScheduleProfile");
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.Discipline", b =>
                {
                    b.HasOne("ScheduleBot.DB.Entity.ClassDTO", "ClassDTO")
                        .WithMany()
                        .HasForeignKey("Class")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ClassDTO");
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.MessageLog", b =>
                {
                    b.HasOne("ScheduleBot.DB.Entity.TelegramUser", "TelegramUser")
                        .WithMany()
                        .HasForeignKey("TelegramUserChatID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TelegramUser");
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.Notifications", b =>
                {
                    b.HasOne("ScheduleBot.DB.Entity.TelegramUser", "TelegramUser")
                        .WithOne()
                        .HasForeignKey("ScheduleBot.DB.Entity.Notifications", "OwnerID");

                    b.Navigation("TelegramUser");
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.ScheduleProfile", b =>
                {
                    b.HasOne("ScheduleBot.DB.Entity.GroupLastUpdate", "GroupLastUpdate")
                        .WithMany()
                        .HasForeignKey("Group");

                    b.HasOne("ScheduleBot.DB.Entity.TelegramUser", "TelegramUser")
                        .WithMany()
                        .HasForeignKey("OwnerID");

                    b.HasOne("ScheduleBot.DB.Entity.StudentIDLastUpdate", "StudentIDLastUpdate")
                        .WithMany()
                        .HasForeignKey("StudentID");

                    b.Navigation("GroupLastUpdate");

                    b.Navigation("StudentIDLastUpdate");

                    b.Navigation("TelegramUser");
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.TeacherWorkSchedule", b =>
                {
                    b.HasOne("ScheduleBot.DB.Entity.ClassDTO", "ClassDTO")
                        .WithMany()
                        .HasForeignKey("Class")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ScheduleBot.DB.Entity.TeacherLastUpdate", "TeacherLastUpdate")
                        .WithMany()
                        .HasForeignKey("Lecturer");

                    b.Navigation("ClassDTO");

                    b.Navigation("TeacherLastUpdate");
                });

            modelBuilder.Entity("ScheduleBot.DB.Entity.TelegramUser", b =>
                {
                    b.HasOne("ScheduleBot.DB.Entity.ModeDTO", "ModeDTO")
                        .WithMany()
                        .HasForeignKey("Mode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ScheduleBot.DB.Entity.Notifications", "Notifications")
                        .WithMany()
                        .HasForeignKey("NotificationsID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ScheduleBot.DB.Entity.ScheduleProfile", "ScheduleProfile")
                        .WithMany()
                        .HasForeignKey("ScheduleProfileGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ModeDTO");

                    b.Navigation("Notifications");

                    b.Navigation("ScheduleProfile");
                });
#pragma warning restore 612, 618
        }
    }
}
