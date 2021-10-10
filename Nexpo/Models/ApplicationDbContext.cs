using Microsoft.EntityFrameworkCore;

namespace Nexpo.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<CompanyConnection> CompanyConnections { get; set; }
        public DbSet<StudentSessionTimeslot> StudentSessionTimeslots { get; set; }
        public DbSet<StudentSessionApplication> StudentSessionApplications { get; set; }
        public DbSet<StudentSession> StudentSessions { get; set; }
    }
}

