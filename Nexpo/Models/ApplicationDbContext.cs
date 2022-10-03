using Microsoft.EntityFrameworkCore;
using Nexpo.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nexpo.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<CompanyConnection> CompanyConnections { get; set; }
        public DbSet<StudentSessionTimeslot> StudentSessionTimeslots { get; set; }
        public DbSet<StudentSessionApplication> StudentSessionApplications { get; set; }
        public DbSet<StudentSession> StudentSessions { get; set; }

        private readonly PasswordService _passwordService;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            PasswordService passwordService) : base(options)
        {
            _passwordService = passwordService;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
            });
        }

        /// <summary>
        /// Seed the database with mock data
        /// </summary>
        public void Seed()
        {
            // Only seed if no data is in the database
            var user = Users.OrderBy(u => u.Id).FirstOrDefault();
            if (user != null)
            {
                Console.WriteLine("Database is already populated, skipping seeding");
                return;
            }
            else
            {
                Console.WriteLine("Seeding Database");
            }

            // Companies
            List<Company> companies = new List<Company>();
            companies.Add( new Company { /* Id = 1, */ Name = "Apple", Description = "A fruit company" });
            companies.Add( new Company { /* Id = 2, */ Name = "Google", Description = "You can find more about us by searching the web" });
            companies.Add( new Company { /* Id = 3, */ Name = "Spotify", Description = "We like music" });
            companies.Add( new Company { /* Id = 4, */ Name = "Facebook", Description = "We have friends in common" });
            foreach(Company c in companies)
            {
                Companies.Add(c);
                SaveChanges();
                Entry(c).GetDatabaseValues();
            }

            // Users
            List<User> users = new List<User>();
            users.Add( new User { /* Id = 1, */ Email = "admin@example.com", PasswordHash = _passwordService.HashPassword("password"), Role = Role.Administrator, FirstName = "Alpha", LastName = "Admin" });
            users.Add( new User { /* Id = 2, */ Email = "student1@example.com", PasswordHash = _passwordService.HashPassword("password"), Role = Role.Student, FirstName = "Alpha", LastName = "Student" });
            users.Add( new User { /* Id = 3, */ Email = "student2@example.com", PasswordHash = _passwordService.HashPassword("password"), Role = Role.Student, FirstName = "Beta", LastName = "Student" });
            users.Add( new User { /* Id = 4, */ Email = "student3@example.com", PasswordHash = _passwordService.HashPassword("password"), Role = Role.Student, FirstName = "Gamma", LastName = "Student" });
            users.Add( new User { /* Id = 5, */ Email = "rep1@company1.example.com", PasswordHash = _passwordService.HashPassword("password"), Role = Role.CompanyRepresentative, FirstName = "Alpha", LastName = "Rep", CompanyId = companies[0].Id.Value });
            users.Add( new User { /* Id = 6, */ Email = "rep2@company1.example.com", PasswordHash = _passwordService.HashPassword("password"), Role = Role.CompanyRepresentative, FirstName = "Beta", LastName = "Rep", CompanyId = companies[0].Id.Value });
            users.Add( new User { /* Id = 7, */ Email = "rep1@company2.example.com", PasswordHash = _passwordService.HashPassword("password"), Role = Role.CompanyRepresentative, FirstName = "Gamma", LastName = "Rep", CompanyId = companies[1].Id.Value });
            users.Add( new User { /* Id = 8, */ Email = "rep1@company3.example.com", PasswordHash = _passwordService.HashPassword("password"), Role = Role.CompanyRepresentative, FirstName = "Delta", LastName = "Rep", CompanyId = companies[2].Id.Value });
            users.Add( new User { /* Id = 9, */ Email = "rep1@company4.example.com", PasswordHash = _passwordService.HashPassword("password"), Role = Role.CompanyRepresentative, FirstName = "Epsilon", LastName = "Rep", CompanyId = companies[3].Id.Value });
            foreach( User u in users)
            {
                Users.Add(u);
                SaveChanges();
                Entry(u).GetDatabaseValues();
            } 

            // Students
            List<Student> students = new List<Student>();
            students.Add( new Student { /* Id = 1, */ Guild = Guild.D, Year = 4, MasterTitle = "Project management in software systems", UserId = users[1].Id.Value });
            students.Add( new Student { /* Id = 2, */ Guild = Guild.I, Year = 2, UserId = users[2].Id.Value, LinkedIn = "my-impressive-profile" });
            students.Add( new Student { /* Id = 3, */ Guild = Guild.V, Year = 3, UserId = users[3].Id.Value });
            foreach( Student s in students)
            {
                Students.Add(s);
                SaveChanges();
                Entry(s).GetDatabaseValues();
            }

            // Events
            List<Event> events = new List<Event>();
            events.Add( new Event { /* Id = 1, */ Name = "Breakfast Mingle", Description = "Breakfast with SEB", Date = "2021-11-12", Start = "08:15", End = "10:00", Host = "SEB", Location = "Cornelis", Language = "Swedish", Capacity = 30 });
            events.Add( new Event { /* Id = 2, */ Name = "Bounce with Uber", Description = "Day event at Bounce with Uber", Date = "2021-11-13", Start = "09:00", End = "16:00", Host = "Uber", Location = "Bounce Malmö", Language = "English", Capacity = 20 });
            events.Add( new Event { /* Id = 3, */ Name = "CV Workshop with Randstad", Description = "Make your CV look professional with the help of Randstad", Date = "2021-11-14", Start = "13:30", End = "15:00", Host = "Randstad", Location = "E:A", Language = "Swedish", Capacity = 3 });
            events.Add( new Event { /* Id = 4, */ Name = "Inspirational lunch lecture", Description = "Get inspired and expand your horizons", Date = "2021-11-15", Start = "12:15", End = "13:00", Host = "SYV", Location = "MA:3", Language = "Swedish", Capacity = 10 });
            foreach(Event e in events)
            {
                Events.Add(e);
                SaveChanges();
                Entry(e).GetDatabaseValues();
            }

            // Tickets
            List<Ticket> tickets = new List<Ticket>();
            tickets.Add( new Ticket { /* Id = 1, */ Code = Guid.NewGuid(), PhotoOk = true, EventId = events[0].Id.Value, UserId = users[1].Id.Value });
            tickets.Add( new Ticket { /* Id = 2, */ Code = Guid.NewGuid(), PhotoOk = false, EventId = events[0].Id.Value, UserId = users[2].Id.Value });
            tickets.Add( new Ticket { /* Id = 3, */ Code = Guid.NewGuid(), PhotoOk = false, EventId = events[1].Id.Value, UserId = users[1].Id.Value });
            tickets.Add( new Ticket { /* Id = 4, */ Code = Guid.NewGuid(), PhotoOk = true, EventId = events[2].Id.Value, UserId = users[2].Id.Value });
            tickets.Add( new Ticket { /* Id = 5, */ Code = Guid.NewGuid(), PhotoOk = true, EventId = events[3].Id.Value, UserId = users[1].Id.Value });
            tickets.Add( new Ticket { /* Id = 6, */ Code = Guid.NewGuid(), PhotoOk = true, EventId = events[3].Id.Value, UserId = users[2].Id.Value });
            tickets.Add( new Ticket { /* Id = 7, */ Code = Guid.NewGuid(), PhotoOk = true, EventId = events[0].Id.Value, UserId = users[3].Id.Value });
            foreach(Ticket t in tickets)
            {
                Tickets.Add(t);
                SaveChanges();
                Entry(t).GetDatabaseValues();
            }

            // StudentSessionTimeslots
            List<StudentSessionTimeslot> studentSessionTimeslots = new List<StudentSessionTimeslot>();
            var session1 = new StudentSessionTimeslot { /* Id = 1, */ Start = DateTime.Parse("2021-11-21 10:00"), End = DateTime.Parse("2021-11-21 10:15"), Location = "Zoom", CompanyId = companies[0].Id.Value };
            var session2 = new StudentSessionTimeslot { /* Id = 1, */ Start = DateTime.Parse("2021-11-21 10:15"), End = DateTime.Parse("2021-11-21 10:30"), Location = "Zoom", CompanyId = companies[0].Id.Value };
            var session3 = new StudentSessionTimeslot { /* Id = 1, */ Start = DateTime.Parse("2021-11-21 10:30"), End = DateTime.Parse("2021-11-21 10:45"), Location = "Zoom", CompanyId = companies[0].Id.Value };
            var session4 = new StudentSessionTimeslot { /* Id = 1, */ Start = DateTime.Parse("2021-11-22 11:00"), End = DateTime.Parse("2021-11-22 11:15"), Location = "Zoom", CompanyId = companies[1].Id.Value };
            var session5 = new StudentSessionTimeslot { /* Id = 1, */ Start = DateTime.Parse("2021-11-22 11:15"), End = DateTime.Parse("2021-11-22 11:30"), Location = "Zoom", CompanyId = companies[1].Id.Value };
            var session6 = new StudentSessionTimeslot { /* Id = 1, */ Start = DateTime.Parse("2021-11-23 12:00"), End = DateTime.Parse("2021-11-22 12:15"), Location = "Zoom", CompanyId = companies[2].Id.Value };
            var session7 = new StudentSessionTimeslot { /* Id = 1, */ Start = DateTime.Parse("2021-11-23 12:15"), End = DateTime.Parse("2021-11-22 12:30"), Location = "Zoom", CompanyId = companies[2].Id.Value };
            studentSessionTimeslots.Add(session1); studentSessionTimeslots.Add(session2); studentSessionTimeslots.Add(session3); studentSessionTimeslots.Add(session4); studentSessionTimeslots.Add(session5); studentSessionTimeslots.Add(session6); studentSessionTimeslots.Add(session7);
            foreach(StudentSessionTimeslot s in studentSessionTimeslots)
            {
                StudentSessionTimeslots.Add(s);
                SaveChanges();
                Entry(s).GetDatabaseValues();
            }

            // StudentSessionApplications
            List<StudentSessionApplication> studentSessionApplications = new List<StudentSessionApplication>();
            var application1 = new StudentSessionApplication { Id = 1, Motivation = "I think you are an interesting company", Rating = 3, StudentId = students[0].Id.Value, CompanyId = companies[0].Id.Value };
            var application2 = new StudentSessionApplication { Id = 2, Motivation = "I love my MacBook", Rating = 4, StudentId = students[1].Id.Value, CompanyId = companies[0].Id.Value };
            var application3 = new StudentSessionApplication { Id = 3, Motivation = "User experience is very important for me", Rating = 5, StudentId = students[2].Id.Value, CompanyId = companies[0].Id.Value };

            var application4 = new StudentSessionApplication { Id = 4, Motivation = "I would like to learn more about searching", Rating = 3, StudentId = students[0].Id.Value, CompanyId = companies[1].Id.Value };
            var application5 = new StudentSessionApplication { Id = 5, Motivation = "I am applying for everything", Rating = 0, StudentId = students[1].Id.Value, CompanyId = companies[1].Id.Value };
            var application6 = new StudentSessionApplication { Id = 6, Motivation = "Search algrorithms are very cool", Rating = 4, StudentId = students[2].Id.Value, CompanyId = companies[1].Id.Value };

            var application7 = new StudentSessionApplication { Id = 7, Motivation = "Music is a big passion of mine", Rating = 4, StudentId = students[1].Id.Value, CompanyId = companies[2].Id.Value };
            studentSessionApplications.Add(application1); studentSessionApplications.Add (application2); studentSessionApplications.Add (application3); studentSessionApplications.Add (application4); studentSessionApplications.Add(application5); studentSessionApplications.Add(application6); studentSessionApplications.Add(application7);
            foreach(var application in studentSessionApplications)
            {
                StudentSessionApplications.Add(application);
                SaveChanges();
                Entry(application).GetDatabaseValues();
            }

            //Student sessions
            List<StudentSession> studentSessions = new List<StudentSession>();
            var studentSession1 = new StudentSession { /*Id = 1, */ StudentId = students[0].Id.Value, Student = students[0], StudentSessionTimeslot = session1, StudentSessionApplication = application1, StudentSessionApplicationId = application1.Id.Value, StudentSessionTimeslotId = session1.Id.Value};
            studentSessions.Add(studentSession1);
            foreach(var session in studentSessions)
            {
                StudentSessions.Add(session);
                SaveChanges();
                Entry(session).GetDatabaseValues();
            }
            
            // CompanyConnections
            List<CompanyConnection> companyConnections = new List<CompanyConnection>();
            var connection1 = new CompanyConnection { Id = 1, Comment = "Someone that is very passionate about what they are doing", Rating = 4, StudentId = students[0].Id.Value, CompanyId = companies[0].Id.Value };
            var connection2 = new CompanyConnection { Id = 2, Comment = "Seems like a interesting guy, contact him later about internship", Rating = 5, StudentId = students[2].Id.Value, CompanyId = companies[3].Id.Value };
            companyConnections.Add(connection1); companyConnections.Add(connection2);
            foreach(var connection in companyConnections)
            {
                CompanyConnections.Add(connection);
                SaveChanges();
                Entry(connection).GetDatabaseValues();
            }
        }
    }
}