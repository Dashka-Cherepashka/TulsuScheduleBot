﻿// <auto-generated />
using System;
using Core.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ScheduleBot.Migrations
{
    [DbContext(typeof(ScheduleDbContext))]
    [Migration("20240923175157_add_DateOfAddition-Discipline")]
    partial class add_DateOfAdditionDiscipline
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Core.DB.Entity.ClassDTO", b =>
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
                        },
                        new
                        {
                            ID = (byte)6,
                            Name = "def"
                        });
                });

            modelBuilder.Entity("Core.DB.Entity.ClassroomLastUpdate", b =>
                {
                    b.Property<string>("Classroom")
                        .HasColumnType("text");

                    b.Property<DateTime>("Update")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("UpdateAttempt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Classroom");

                    b.ToTable("ClassroomLastUpdate");
                });

            modelBuilder.Entity("Core.DB.Entity.ClassroomWorkSchedule", b =>
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

                    b.HasIndex("LectureHall");

                    b.HasIndex("Lecturer");

                    b.ToTable("ClassroomWorkSchedule");
                });

            modelBuilder.Entity("Core.DB.Entity.CompletedDiscipline", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ID"));

                    b.Property<byte>("Class")
                        .HasColumnType("smallint");

                    b.Property<DateOnly?>("Date")
                        .HasColumnType("date");

                    b.Property<string>("IntersectionMark")
                        .HasColumnType("text");

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

            modelBuilder.Entity("Core.DB.Entity.CustomDiscipline", b =>
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

            modelBuilder.Entity("Core.DB.Entity.DeletedDisciplines", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ID"));

                    b.Property<byte>("Class")
                        .HasColumnType("smallint");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<DateOnly>("DeleteDate")
                        .HasColumnType("date");

                    b.Property<TimeOnly>("EndTime")
                        .HasColumnType("time without time zone");

                    b.Property<string>("Group")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("IntersectionMark")
                        .HasColumnType("text");

                    b.Property<string>("LectureHall")
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

                    b.HasIndex("Date");

                    b.HasIndex("Group");

                    b.HasIndex("Lecturer");

                    b.ToTable("DeletedDisciplines");
                });

            modelBuilder.Entity("Core.DB.Entity.Discipline", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ID"));

                    b.Property<byte>("Class")
                        .HasColumnType("smallint");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<DateOnly?>("DateOfAddition")
                        .HasColumnType("date");

                    b.Property<TimeOnly>("EndTime")
                        .HasColumnType("time without time zone");

                    b.Property<string>("Group")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("IntersectionMark")
                        .HasColumnType("text");

                    b.Property<string>("LectureHall")
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

                    b.HasIndex("Date");

                    b.HasIndex("Group");

                    b.HasIndex("LectureHall");

                    b.HasIndex("Lecturer");

                    b.ToTable("Disciplines");
                });

            modelBuilder.Entity("Core.DB.Entity.Feedback", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ID"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("From")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("From");

                    b.ToTable("Feedbacks");
                });

            modelBuilder.Entity("Core.DB.Entity.GroupLastUpdate", b =>
                {
                    b.Property<string>("Group")
                        .HasColumnType("text");

                    b.Property<DateTime>("Update")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("UpdateAttempt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Group");

                    b.ToTable("GroupLastUpdate");
                });

            modelBuilder.Entity("Core.DB.Entity.IntersectionOfSubgroups", b =>
                {
                    b.Property<string>("Group")
                        .HasColumnType("text");

                    b.Property<byte>("Class")
                        .HasColumnType("smallint");

                    b.Property<string>("IntersectionWith")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Mark")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Group");

                    b.HasIndex("Class");

                    b.HasIndex("IntersectionWith")
                        .IsUnique();

                    b.ToTable("IntersectionOfSubgroups");
                });

            modelBuilder.Entity("Core.DB.Entity.MessageLog", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ID"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("From")
                        .HasColumnType("bigint");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Request")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("From");

                    b.ToTable("MessageLog");
                });

            modelBuilder.Entity("Core.DB.Entity.Messenger", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ID"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long?>("FeedbackID")
                        .HasColumnType("bigint");

                    b.Property<long?>("Following")
                        .HasColumnType("bigint");

                    b.Property<long>("From")
                        .HasColumnType("bigint");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("Previous")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("FeedbackID");

                    b.HasIndex("Following");

                    b.HasIndex("From");

                    b.HasIndex("Previous");

                    b.ToTable("Messenger");
                });

            modelBuilder.Entity("Core.DB.Entity.MissingFields", b =>
                {
                    b.Property<string>("Field")
                        .HasColumnType("text");

                    b.HasKey("Field");

                    b.ToTable("MissingFields");
                });

            modelBuilder.Entity("Core.DB.Entity.ModeDTO", b =>
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
                        },
                        new
                        {
                            ID = (byte)14,
                            Name = "ClassroomSchedule"
                        },
                        new
                        {
                            ID = (byte)15,
                            Name = "ClassroomSelected"
                        },
                        new
                        {
                            ID = (byte)16,
                            Name = "Feedback"
                        },
                        new
                        {
                            ID = (byte)17,
                            Name = "Messenger"
                        },
                        new
                        {
                            ID = (byte)18,
                            Name = "Admin"
                        },
                        new
                        {
                            ID = (byte)19,
                            Name = "Dispatch"
                        });
                });

            modelBuilder.Entity("Core.DB.Entity.Progress", b =>
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

                    b.HasIndex("StudentID");

                    b.ToTable("Progresses");
                });

            modelBuilder.Entity("Core.DB.Entity.ScheduleProfile", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Group")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastAppeal")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("OwnerID")
                        .HasColumnType("bigint");

                    b.Property<string>("StudentID")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("Group");

                    b.HasIndex("StudentID");

                    b.ToTable("ScheduleProfile");
                });

            modelBuilder.Entity("Core.DB.Entity.Settings", b =>
                {
                    b.Property<long>("OwnerID")
                        .HasColumnType("bigint");

                    b.Property<bool>("DisplayingGroupList")
                        .HasColumnType("boolean");

                    b.Property<int>("NotificationDays")
                        .HasColumnType("integer");

                    b.Property<bool>("NotificationEnabled")
                        .HasColumnType("boolean");

                    b.Property<bool>("TeacherLincsEnabled")
                        .HasColumnType("boolean");

                    b.HasKey("OwnerID");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("Core.DB.Entity.StudentIDLastUpdate", b =>
                {
                    b.Property<string>("StudentID")
                        .HasColumnType("text");

                    b.Property<DateTime>("Update")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("UpdateAttempt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("StudentID");

                    b.ToTable("StudentIDLastUpdate");
                });

            modelBuilder.Entity("Core.DB.Entity.TeacherLastUpdate", b =>
                {
                    b.Property<string>("Teacher")
                        .HasColumnType("text");

                    b.Property<string>("LinkProfile")
                        .HasColumnType("text");

                    b.Property<DateTime>("Update")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("UpdateAttempt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Teacher");

                    b.ToTable("TeacherLastUpdate");
                });

            modelBuilder.Entity("Core.DB.Entity.TeacherWorkSchedule", b =>
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
                        .IsRequired()
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

                    b.HasIndex("LectureHall");

                    b.HasIndex("Lecturer");

                    b.ToTable("TeacherWorkSchedule");
                });

            modelBuilder.Entity("Core.DB.Entity.TelegramUser", b =>
                {
                    b.Property<long>("ChatID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ChatID"));

                    b.Property<DateTime?>("DateOfRegistration")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDeactivated")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastAppeal")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<Guid>("ScheduleProfileGuid")
                        .HasColumnType("uuid");

                    b.Property<long>("TodayRequests")
                        .HasColumnType("bigint");

                    b.Property<long>("TotalRequests")
                        .HasColumnType("bigint");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("ChatID");

                    b.HasIndex("ScheduleProfileGuid");

                    b.ToTable("TelegramUsers");
                });

            modelBuilder.Entity("Core.DB.Entity.TelegramUsersTmp", b =>
                {
                    b.Property<long>("OwnerID")
                        .HasColumnType("bigint");

                    b.Property<byte>("Mode")
                        .HasColumnType("smallint");

                    b.Property<string>("TmpData")
                        .HasColumnType("text");

                    b.HasKey("OwnerID");

                    b.HasIndex("Mode");

                    b.ToTable("TelegramUsersTmp");
                });

            modelBuilder.Entity("Core.DB.Entity.ClassroomWorkSchedule", b =>
                {
                    b.HasOne("Core.DB.Entity.ClassDTO", "ClassDTO")
                        .WithMany()
                        .HasForeignKey("Class")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.DB.Entity.ClassroomLastUpdate", "ClassroomLastUpdate")
                        .WithMany()
                        .HasForeignKey("LectureHall")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.DB.Entity.TeacherLastUpdate", "TeacherLastUpdate")
                        .WithMany()
                        .HasForeignKey("Lecturer");

                    b.Navigation("ClassDTO");

                    b.Navigation("ClassroomLastUpdate");

                    b.Navigation("TeacherLastUpdate");
                });

            modelBuilder.Entity("Core.DB.Entity.CompletedDiscipline", b =>
                {
                    b.HasOne("Core.DB.Entity.ClassDTO", "ClassDTO")
                        .WithMany()
                        .HasForeignKey("Class")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.DB.Entity.ScheduleProfile", "ScheduleProfile")
                        .WithMany()
                        .HasForeignKey("ScheduleProfileGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ClassDTO");

                    b.Navigation("ScheduleProfile");
                });

            modelBuilder.Entity("Core.DB.Entity.CustomDiscipline", b =>
                {
                    b.HasOne("Core.DB.Entity.ScheduleProfile", "ScheduleProfile")
                        .WithMany()
                        .HasForeignKey("ScheduleProfileGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ScheduleProfile");
                });

            modelBuilder.Entity("Core.DB.Entity.DeletedDisciplines", b =>
                {
                    b.HasOne("Core.DB.Entity.ClassDTO", "ClassDTO")
                        .WithMany()
                        .HasForeignKey("Class")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.DB.Entity.GroupLastUpdate", "GroupLastUpdate")
                        .WithMany()
                        .HasForeignKey("Group")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.DB.Entity.TeacherLastUpdate", "TeacherLastUpdate")
                        .WithMany()
                        .HasForeignKey("Lecturer");

                    b.Navigation("ClassDTO");

                    b.Navigation("GroupLastUpdate");

                    b.Navigation("TeacherLastUpdate");
                });

            modelBuilder.Entity("Core.DB.Entity.Discipline", b =>
                {
                    b.HasOne("Core.DB.Entity.ClassDTO", "ClassDTO")
                        .WithMany()
                        .HasForeignKey("Class")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.DB.Entity.GroupLastUpdate", "GroupLastUpdate")
                        .WithMany()
                        .HasForeignKey("Group")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.DB.Entity.ClassroomLastUpdate", "ClassroomLastUpdate")
                        .WithMany()
                        .HasForeignKey("LectureHall");

                    b.HasOne("Core.DB.Entity.TeacherLastUpdate", "TeacherLastUpdate")
                        .WithMany()
                        .HasForeignKey("Lecturer");

                    b.Navigation("ClassDTO");

                    b.Navigation("ClassroomLastUpdate");

                    b.Navigation("GroupLastUpdate");

                    b.Navigation("TeacherLastUpdate");
                });

            modelBuilder.Entity("Core.DB.Entity.Feedback", b =>
                {
                    b.HasOne("Core.DB.Entity.TelegramUser", "TelegramUser")
                        .WithMany()
                        .HasForeignKey("From")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TelegramUser");
                });

            modelBuilder.Entity("Core.DB.Entity.IntersectionOfSubgroups", b =>
                {
                    b.HasOne("Core.DB.Entity.ClassDTO", "ClassDTO")
                        .WithMany()
                        .HasForeignKey("Class")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ClassDTO");
                });

            modelBuilder.Entity("Core.DB.Entity.MessageLog", b =>
                {
                    b.HasOne("Core.DB.Entity.TelegramUser", "TelegramUser")
                        .WithMany()
                        .HasForeignKey("From")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TelegramUser");
                });

            modelBuilder.Entity("Core.DB.Entity.Messenger", b =>
                {
                    b.HasOne("Core.DB.Entity.Feedback", "Feedback")
                        .WithMany()
                        .HasForeignKey("FeedbackID");

                    b.HasOne("Core.DB.Entity.Messenger", "FollowingMessenger")
                        .WithMany()
                        .HasForeignKey("Following");

                    b.HasOne("Core.DB.Entity.TelegramUser", "TelegramUser")
                        .WithMany()
                        .HasForeignKey("From")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.DB.Entity.Messenger", "PreviousMessenger")
                        .WithMany()
                        .HasForeignKey("Previous");

                    b.Navigation("Feedback");

                    b.Navigation("FollowingMessenger");

                    b.Navigation("PreviousMessenger");

                    b.Navigation("TelegramUser");
                });

            modelBuilder.Entity("Core.DB.Entity.Progress", b =>
                {
                    b.HasOne("Core.DB.Entity.StudentIDLastUpdate", "StudentIDLastUpdate")
                        .WithMany()
                        .HasForeignKey("StudentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("StudentIDLastUpdate");
                });

            modelBuilder.Entity("Core.DB.Entity.ScheduleProfile", b =>
                {
                    b.HasOne("Core.DB.Entity.GroupLastUpdate", "GroupLastUpdate")
                        .WithMany()
                        .HasForeignKey("Group");

                    b.HasOne("Core.DB.Entity.StudentIDLastUpdate", "StudentIDLastUpdate")
                        .WithMany()
                        .HasForeignKey("StudentID");

                    b.Navigation("GroupLastUpdate");

                    b.Navigation("StudentIDLastUpdate");
                });

            modelBuilder.Entity("Core.DB.Entity.Settings", b =>
                {
                    b.HasOne("Core.DB.Entity.TelegramUser", "TelegramUser")
                        .WithOne("Settings")
                        .HasForeignKey("Core.DB.Entity.Settings", "OwnerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TelegramUser");
                });

            modelBuilder.Entity("Core.DB.Entity.TeacherWorkSchedule", b =>
                {
                    b.HasOne("Core.DB.Entity.ClassDTO", "ClassDTO")
                        .WithMany()
                        .HasForeignKey("Class")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.DB.Entity.ClassroomLastUpdate", "ClassroomLastUpdate")
                        .WithMany()
                        .HasForeignKey("LectureHall")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.DB.Entity.TeacherLastUpdate", "TeacherLastUpdate")
                        .WithMany()
                        .HasForeignKey("Lecturer")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ClassDTO");

                    b.Navigation("ClassroomLastUpdate");

                    b.Navigation("TeacherLastUpdate");
                });

            modelBuilder.Entity("Core.DB.Entity.TelegramUser", b =>
                {
                    b.HasOne("Core.DB.Entity.ScheduleProfile", "ScheduleProfile")
                        .WithMany()
                        .HasForeignKey("ScheduleProfileGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ScheduleProfile");
                });

            modelBuilder.Entity("Core.DB.Entity.TelegramUsersTmp", b =>
                {
                    b.HasOne("Core.DB.Entity.ModeDTO", "ModeDTO")
                        .WithMany()
                        .HasForeignKey("Mode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.DB.Entity.TelegramUser", "TelegramUser")
                        .WithOne("TelegramUserTmp")
                        .HasForeignKey("Core.DB.Entity.TelegramUsersTmp", "OwnerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ModeDTO");

                    b.Navigation("TelegramUser");
                });

            modelBuilder.Entity("Core.DB.Entity.TelegramUser", b =>
                {
                    b.Navigation("Settings")
                        .IsRequired();

                    b.Navigation("TelegramUserTmp")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
