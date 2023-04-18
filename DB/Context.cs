﻿using Microsoft.EntityFrameworkCore;

using ScheduleBot.DB.Entity;

namespace ScheduleBot.DB {
    public class ScheduleDbContext : DbContext {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseNpgsql("Host=localhost;Database=ScheduleDB;Username=postgres;Password=1901");

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            foreach(Entity.Type type in Enum.GetValues(typeof(Entity.Type)).Cast<Entity.Type>())
                modelBuilder.Entity<TypeDTO>().HasData(new TypeDTO() { ID = type, Name = type.ToString() });

            foreach(Entity.Mode type in Enum.GetValues(typeof(Entity.Mode)).Cast<Entity.Mode>())
                modelBuilder.Entity<ModeDTO>().HasData(new ModeDTO() { ID = type, Name = type.ToString() });
        }

        public void CleanDB() {
            Database.EnsureDeleted();
            Database.EnsureCreated();
            SaveChanges();
        }

        public IQueryable<Discipline> GetDisciplinesBetweenDates((DateOnly min, DateOnly max) dates) => Disciplines.Where(i => i.Date >= dates.min && i.Date <= dates.max);

#pragma warning disable CS8618
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Progress> Progresses { get; set; }
        public DbSet<CompletedDiscipline> CompletedDisciplines { get; set; }
        public DbSet<TypeDTO> Types { get; set; }
        public DbSet<ModeDTO> Modes { get; set; }
        public DbSet<TelegramUser> TelegramUsers { get; set; }
        public DbSet<TemporaryAddition> TemporaryAddition { get; set; }
    }
}
