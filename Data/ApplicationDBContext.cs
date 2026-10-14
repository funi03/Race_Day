using Microsoft.EntityFrameworkCore;
using Race_Day.Models;

namespace Race_Day.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Organiser> Organisers { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<EventEnrolment> EventEnrolments { get; set; }
        public DbSet<Result> Results { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ---- Explicit primary keys ----
            modelBuilder.Entity<User>().HasKey(u => u.UserID);
            modelBuilder.Entity<Organiser>().HasKey(o => o.OrganiserID);
            modelBuilder.Entity<Participant>().HasKey(p => p.ParticipantID);
            modelBuilder.Entity<Event>().HasKey(e => e.EventID);
            modelBuilder.Entity<Category>().HasKey(c => c.CategoryID);
            modelBuilder.Entity<EventEnrolment>().HasKey(e => e.EnrolmentID);
            modelBuilder.Entity<Result>().HasKey(r => r.ResultID);

            // ---- Decimal precision (matches SQL script DECIMAL(6,2)) ----
            modelBuilder.Entity<Event>()
                .Property(e => e.Distance)
                .HasPrecision(6, 2);

            modelBuilder.Entity<Category>()
                .Property(c => c.DistanceKm)
                .HasPrecision(6, 2);

            // ---- One-to-one: User -> Organiser ----
            modelBuilder.Entity<Organiser>()
                .HasOne(o => o.User)
                .WithOne(u => u.Organiser)
                .HasForeignKey<Organiser>(o => o.UserID);

            // ---- One-to-one: User -> Participant ----
            modelBuilder.Entity<Participant>()
                .HasOne(p => p.User)
                .WithOne(u => u.Participant)
                .HasForeignKey<Participant>(p => p.UserID);

            // ---- One-to-one: Enrolment -> Result ----
            modelBuilder.Entity<Result>()
                .HasOne(r => r.Enrolment)
                .WithOne(e => e.Result)
                .HasForeignKey<Result>(r => r.EnrolmentID);

            // ---- Unique email ----
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}