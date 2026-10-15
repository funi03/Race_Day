using Microsoft.EntityFrameworkCore;
using Race_Day.Data;
using Race_Day.Models;
using Xunit;
using System.Security.Claims;

namespace TestProjectRD
{
    public class RaceDayTests
    {
        private ApplicationDBContext GetContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDBContext(options);
        }

        [Fact]
        public async Task Register_NewUser_ReturnsOk()
        {
            using var ctx = GetContext();
            ctx.Users.Add(new User
            {
                FullName = "Test", Email = "test@x.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pass123!"),
                Role = "Organiser"
            });
            await ctx.SaveChangesAsync();
            Assert.Equal(1, await ctx.Users.CountAsync());
        }

        [Fact]
        public async Task Register_DuplicateEmail_IsDetected()
        {
            using var ctx = GetContext();
            ctx.Users.Add(new User
            {
                FullName = "A", Email = "dup@x.com",
                PasswordHash = "h", Role = "Participant"
            });
            await ctx.SaveChangesAsync();
            Assert.True(await ctx.Users.AnyAsync(u => u.Email == "dup@x.com"));
        }

        [Fact]
        public void Login_CorrectPassword_VerifiesTrue()
        {
            var hash = BCrypt.Net.BCrypt.HashPassword("Secret123");
            Assert.True(BCrypt.Net.BCrypt.Verify("Secret123", hash));
        }

        [Fact]
        public void Login_WrongPassword_VerifiesFalse()
        {
            var hash = BCrypt.Net.BCrypt.HashPassword("Secret123");
            Assert.False(BCrypt.Net.BCrypt.Verify("WrongPass", hash));
        }

        [Fact]
        public async Task CreateEvent_AsOrganiser_Succeeds()
        {
            using var ctx = GetContext();
            ctx.Events.Add(new Event
            {
                OrganiserID = 1, EventName = "Test Run",
                Description = "d", EventDate = DateTime.Today.AddDays(30),
                Location = "JHB", Distance = 10m, EventType = "Run"
            });
            await ctx.SaveChangesAsync();
            Assert.Equal(1, await ctx.Events.CountAsync());
        }

        [Fact]
        public void Participant_CannotCreateEvent_RoleCheckFails()
        {
            var claims = new[] { new Claim(ClaimTypes.Role, "Participant") };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
            Assert.False(principal.IsInRole("Organiser"));
        }

        [Fact]
        public async Task Enrol_Participant_RecordsEnrolment()
        {
            using var ctx = GetContext();
            ctx.EventEnrolments.Add(new EventEnrolment
            {
                ParticipantID = 1, EventID = 1, CategoryID = 1,
                Status = "Registered"
            });
            await ctx.SaveChangesAsync();
            Assert.Equal("Registered", (await ctx.EventEnrolments.FirstAsync()).Status);
        }

        [Fact]
        public void Organiser_CannotEnrol_RoleCheckFails()
        {
            var claims = new[] { new Claim(ClaimTypes.Role, "Organiser") };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
            Assert.False(principal.IsInRole("Participant"));
        }
    }
}
