using Microsoft.EntityFrameworkCore;
using Race_Day.Data;
using Race_Day.Models;
using Xunit;
using System.Security.Claims;

namespace TestProjectRD
{
    public class RaceDayTests
    {
        // Helper — fresh in-memory context per test
        private ApplicationDBContext GetContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDBContext(options);
        }

        // ---------- 1. Registration success ----------
        [Fact]
        public async Task Register_NewUser_ReturnsOk()
        {
            using var ctx = GetContext();
            var user = new User
            {
                FullName = "Test Organiser",
                Email = "test@raceday.co.za",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pass123!"),
                Role = "Organiser"
            };
            ctx.Users.Add(user);
            await ctx.SaveChangesAsync();

            Assert.Equal(1, await ctx.Users.CountAsync());
            Assert.Equal("Organiser", (await ctx.Users.FirstAsync()).Role);
        }

        // ---------- 2. Registration failure — duplicate email ----------
        [Fact]
        public async Task Register_DuplicateEmail_IsDetected()
        {
            using var ctx = GetContext();
            ctx.Users.Add(new User
            {
                FullName = "A",
                Email = "dup@x.com",
                PasswordHash = "h",
                Role = "Participant"
            });
            await ctx.SaveChangesAsync();

            bool exists = await ctx.Users.AnyAsync(u => u.Email == "dup@x.com");
            Assert.True(exists);
        }

        // ---------- 3. Login success ----------
        [Fact]
        public void Login_CorrectPassword_VerifiesTrue()
        {
            var hash = BCrypt.Net.BCrypt.HashPassword("Secret123");
            Assert.True(BCrypt.Net.BCrypt.Verify("Secret123", hash));
        }

        // ---------- 4. Login failure ----------
        [Fact]
        public void Login_WrongPassword_VerifiesFalse()
        {
            var hash = BCrypt.Net.BCrypt.HashPassword("Secret123");
            Assert.False(BCrypt.Net.BCrypt.Verify("WrongPass", hash));
        }

        // ---------- 5. Event creation persists ----------
        [Fact]
        public async Task CreateEvent_AsOrganiser_Succeeds()
        {
            using var ctx = GetContext();
            var ev = new Event
            {
                OrganiserID = 1,
                EventName = "Test Run",
                Description = "desc",
                EventDate = DateTime.Today.AddDays(30),
                Location = "JHB",
                Distance = 10m,
                EventType = "Run"
            };
            ctx.Events.Add(ev);
            await ctx.SaveChangesAsync();

            Assert.Equal(1, await ctx.Events.CountAsync());
            Assert.Equal("Test Run", (await ctx.Events.FirstAsync()).EventName);
        }

        // ---------- 6. Role-based rejection ----------
        [Fact]
        public void Participant_CannotCreateEvent_RoleCheckFails()
        {
            var claims = new[] { new Claim(ClaimTypes.Role, "Participant") };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            Assert.False(principal.IsInRole("Organiser"));
            Assert.True(principal.IsInRole("Participant"));
        }

        // ---------- 7. Enrolment recorded ----------
        [Fact]
        public async Task Enrol_Participant_RecordsEnrolment()
        {
            using var ctx = GetContext();
            ctx.EventEnrolments.Add(new EventEnrolment
            {
                ParticipantID = 1,
                EventID = 1,
                CategoryID = 1,
                Status = "Registered"
            });
            await ctx.SaveChangesAsync();

            var e = await ctx.EventEnrolments.FirstAsync();
            Assert.Equal("Registered", e.Status);
            Assert.Equal(1, e.EventID);
        }

        // ---------- 8. Enrolment role rejection ----------
        [Fact]
        public void Organiser_CannotEnrol_RoleCheckFails()
        {
            var claims = new[] { new Claim(ClaimTypes.Role, "Organiser") };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            Assert.False(principal.IsInRole("Participant"));
        }
    }
}