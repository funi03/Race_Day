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

            // ---- EventEnrolment relationships (Restrict to avoid cascade cycles) ----
            modelBuilder.Entity<EventEnrolment>()
                .HasOne(e => e.Participant)
                .WithMany(p => p.Enrolments)
                .HasForeignKey(e => e.ParticipantID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventEnrolment>()
                .HasOne(e => e.Event)
                .WithMany(ev => ev.Enrolments)
                .HasForeignKey(e => e.EventID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventEnrolment>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Enrolments)
                .HasForeignKey(e => e.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            // ---- One-to-one: Enrolment -> Result (Restrict) ----
            modelBuilder.Entity<Result>()
                .HasOne(r => r.Enrolment)
                .WithOne(e => e.Result)
                .HasForeignKey<Result>(r => r.EnrolmentID)
                .OnDelete(DeleteBehavior.Restrict);

            // ---- Unique email ----
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}