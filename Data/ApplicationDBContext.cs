using Microsoft.EntityFrameworkCore;
using Race_Day.Models;

namespace Race_Day.Data
{
        public class ApplicationDBContext : DbContext
        {//start class
            public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Organisers> Organisers { get; set; }
        public DbSet<Participants> Participants { get; set; }
        public DbSet<Events> Events { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<EventEnrolments> EventEnrolments { get; set; }
        public DbSet<Results> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Enforce one-to-one: User -> Organiser / Participant
            modelBuilder.Entity<Organiser>()
                .HasOne(o => o.User)
                .WithOne(u => u.Organiser)
                .HasForeignKey<Organiser>(o => o.UserID);

            modelBuilder.Entity<Participant>()
                .HasOne(p => p.User)
                .WithOne(u => u.Participant)
                .HasForeignKey<Participant>(p => p.UserID);

            // Enforce one-to-one: Enrolment -> Result
            modelBuilder.Entity<Result>()
                .HasOne(r => r.Enrolment)
                .WithOne(e => e.Result)
                .HasForeignKey<Result>(r => r.EnrolmentID);

            // Unique email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}

