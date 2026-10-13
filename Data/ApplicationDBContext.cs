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
        public DbSet<results> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Enforce one-to-one: User -> Organiser / Participant
            modelBuilder.Entity<Organisers>()
                .HasOne(o => o.User)
                .WithOne(u => u.Organiser)
                .HasForeignKey<Organisers>(o => o.UserID);

            modelBuilder.Entity<Participants>()
                .HasOne(p => p.User)
                .WithOne(u => u.Participant)
                .HasForeignKey<Participants>(p => p.UserID);

            // Enforce one-to-one: Enrolment -> Result
            modelBuilder.Entity<results>()
                .HasOne(r => r.Enrolment)
                .WithOne(e => e.Result)
                .HasForeignKey<results>(r => r.EnrolmentID);

            // Unique email
            modelBuilder.Entity<Users>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}

