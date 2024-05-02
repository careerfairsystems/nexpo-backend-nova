using Microsoft.EntityFrameworkCore;
using Nexpo.Services;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Nexpo.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<StudentSessionTimeslot> StudentSessionTimeslots { get; set; }
        public DbSet<StudentSessionApplication> StudentSessionApplications { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<FrequentAskedQuestion> FrequentAskedQuestion { get; set; }

        public DbSet<NotificationTopic> NotificationTopic { get; set; }

#pragma warning restore format

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
            var company1 = new Company { Id = -1, Name = "Apple", Description = "A fruit company" , DidYouKnow = "Apples", DaysAtArkad = new List<DateTime>(){DateTime.Parse("2023-01-01"), DateTime.Parse("2023-02-02"), DateTime.Parse("2023-03-03")}, DesiredDegrees = new List<int>(){(int) Degree.Bachelor,(int) Degree.Master}, DesiredProgramme = new List<int>(){(int) Programme.Datateknik,(int) Programme.Elektroteknik}, Industries = new List<int>(){(int)Industry.DataIT,(int) Industry.ElectricityEnergyPower}, Positions = new List<int>(){(int)Position.ForeignOppurtunity, (int)Position.Internship}, StudentSessionMotivation = "We are the greatest company in the world according to us!"};
            var company2 = new Company { Id = -2, Name = "Google", Description = "You can find more about us by searching the web"  , DidYouKnow = "we are big", DaysAtArkad = new List<DateTime>(){DateTime.Parse("2023-01-01")}, DesiredDegrees = new List<int>(){(int) Degree.Bachelor,(int) Degree.Master}, DesiredProgramme = new List<int>(){(int) Programme.Arkitekt,(int) Programme.Väg_och_vatttenbyggnad}, Industries = new List<int>(){(int)Industry.Industry,(int) Industry.DataIT}, Positions = new List<int>(){(int)Position.PartTime, (int)Position.Internship}};
            var company3 = new Company { Id = -3, Name = "Spotify", Description = "We like music" ,  DidYouKnow = "we love music", DaysAtArkad = new List<DateTime>(){DateTime.Parse("2023-01-01"), DateTime.Parse("2023-01-02")}, DesiredDegrees = new List<int>(){(int) Degree.Bachelor,(int) Degree.Master}, DesiredProgramme = new List<int>(){(int) Programme.Kemiteknik,(int) Programme.Industriell_ekonomi}, Industries = new List<int>(){(int)Industry.Coaching,(int) Industry.BankingFinance}, Positions = new List<int>(){(int)Position.ForeignOppurtunity, (int)Position.SummerJob}};
            var company4 = new Company { Id = -4, Name = "Facebook", Description = "We have friends in common" , DidYouKnow = "Mark zuckerburg is an Alien", DaysAtArkad = new List<DateTime>(){DateTime.Parse("2023-01-01"), DateTime.Parse("2023-01-02")}, DesiredDegrees = new List<int>(){(int) Degree.PhD,(int) Degree.Master}, DesiredProgramme = new List<int>(){(int) Programme.Byggteknik_med_Järnvägsteknik,(int) Programme.Teknisk_Fysik}, Industries = new List<int>(){(int)Industry.Environment,(int) Industry.ElectricityEnergyPower}, Positions = new List<int>(){(int)Position.Thesis, (int)Position.TraineeEmployment}, StudentSessionMotivation = "We are better than Apple!"};
            Companies.AddRange(company1, company2, company3, company4);
            SaveChanges();

            // Users
            var user1 = new User { Id = -1, Email = "admin@example.com", PasswordHash = _passwordService.HashPassword("password"), Role = Role.Administrator, FirstName = "Alpha", LastName = "Admin" };

            var user2 = new User { Id = -2, Email = "student1@example.com", PasswordHash = _passwordService.HashPassword("password"), Role = Role.Student, FirstName = "Alpha", LastName = "Student" };
            var user3 = new User { Id = -3, Email = "student2@example.com", PasswordHash = _passwordService.HashPassword("password"), Role = Role.Student, FirstName = "Beta", LastName = "Student" };
            var user4 = new User { Id = -4, Email = "student3@example.com", PasswordHash = _passwordService.HashPassword("password"), Role = Role.Student, FirstName = "Gamma", LastName = "Student" };

            var user5 = new User { Id = -5, Email = "rep1@company1.example.com", PasswordHash = _passwordService.HashPassword("password"), Role = Role.CompanyRepresentative, FirstName = "Alpha", LastName = "Rep", CompanyId = company1.Id.Value };
            var user6 = new User { Id = -6, Email = "rep2@company1.example.com", PasswordHash = _passwordService.HashPassword("password"), Role = Role.CompanyRepresentative, FirstName = "Beta", LastName = "Rep", CompanyId = company1.Id.Value };
            var user7 = new User { Id = -7, Email = "rep1@company2.example.com", PasswordHash = _passwordService.HashPassword("password"), Role = Role.CompanyRepresentative, FirstName = "Gamma", LastName = "Rep", CompanyId = company2.Id.Value };
            var user8 = new User { Id = -8, Email = "rep1@company3.example.com", PasswordHash = _passwordService.HashPassword("password"), Role = Role.CompanyRepresentative, FirstName = "Delta", LastName = "Rep", CompanyId = company3.Id.Value };
            var user9 = new User { Id = -9, Email = "rep1@company4.example.com", PasswordHash = _passwordService.HashPassword("password"), Role = Role.CompanyRepresentative, FirstName = "Epsilon", LastName = "Rep", CompanyId = company4.Id.Value };

            var user10 = new User { Id = -10, Email = "volunteer@example.com", PasswordHash = _passwordService.HashPassword("password"), Role = Role.Volunteer, FirstName = "Alpha", LastName = "Volunteer" };
            Users.AddRange(user1, user2, user3, user4, user5, user6, user7, user8, user9, user10);
            SaveChanges();

            // Students
            var student1 = new Student { Id = -1, Programme = Programme.Datateknik, Year = 4, MasterTitle = "Project management in software systems", UserId = user2.Id.Value };
            var student2 = new Student { Id = -2, Programme = Programme.Industriell_ekonomi, Year = 2, UserId = user3.Id.Value, LinkedIn = "my-impressive-profile" };
            var student3 = new Student { Id = -3, Programme = Programme.Väg_och_vatttenbyggnad, Year = 3, UserId = user4.Id.Value };
            Students.AddRange(student1, student2, student3);
            SaveChanges();

            // Volunteers
            var volunteer1 = new Volunteer { Id = -10, Programme = Programme.Datateknik, Year = 4, MasterTitle = "Project management in software systems", UserId = user10.Id.Value };
            Volunteers.AddRange(volunteer1);
            SaveChanges();

            // Events
            var event1 = new Event { Id = -1, Name = "Breakfast Mingle",            Type = EventType.CompanyEvent, Description = "Breakfast with SEB",                                       Date = DateTime.Now.AddDays(10).Date.ToString(), Start = "08:15", End = "10:00", Host = "SEB",           Location = "Cornelis",     Language = "Swedish", Capacity = 30 };
            var event2 = new Event { Id = -2, Name = "Bounce with Uber",            Type = EventType.CompanyEvent, Description = "Day event at Bounce with Uber",                            Date = DateTime.Now.AddDays(11).Date.ToString(), Start = "09:00", End = "16:00", Host = "Uber",          Location = "Bounce Malmö", Language = "English", Capacity = 20 };
            var event3 = new Event { Id = -3, Name = "CV Workshop with Randstad",   Type = EventType.CompanyEvent, Description = "Make your CV look professional with the help of Randstad", Date = DateTime.Now.AddDays(12).Date.ToString(), Start = "13:30", End = "15:00", Host = "Randstad",      Location = "E:A",          Language = "Swedish", Capacity = 3 };
            var event4 = new Event { Id = -4, Name = "Inspirational lunch lecture", Type = EventType.CompanyEvent, Description = "Get inspired and expand your horizons",                    Date = DateTime.Now.AddDays(14).Date.ToString(), Start = "12:15", End = "13:00", Host = "SYV",           Location = "MA:3",         Language = "Swedish", Capacity = 2 };
            var event5 = new Event { Id = -5, Name = "Pick apples with Apple",      Type = EventType.CompanyEvent, Description = "An apple a day keeps the doctor away",                     Date = DateTime.Now.AddDays(1).Date.ToString(),  Start = "12:15", End = "13:00", Host = "Apple",         Location = "M:B",          Language = "English", Capacity = 200};
            var event6 = new Event { Id = -6, Name = "Lunch For volunteers",        Type = EventType.Lunch,         Description = "A lunch for all volunteers to enjoy",                      Date = DateTime.Now.AddDays(15).Date.ToString(), Start = "12:15", End = "13:00", Host = "Teknologkåren", Location = "Hänget",   Language = "Swrdish", Capacity = 300};
            var event7 = new Event { Id = -7, Name = "Lunch For Company Rep!",      Type = EventType.Lunch,         Description = "Com.reps get hungry aswell",                               Date = DateTime.Now.AddDays(15).Date.ToString(), Start = "12:15", End = "13:00", Host = "Teknologkåren", Location = "Perstorp", Language = "English", Capacity = 500};

            var event8 = new Event { Id = -8, Name = "The final banquet",           Type = EventType.Banquet,       Description = "The final banquet for all volunteers, company hosts, and company reps", Date = DateTime.Now.AddDays(15).Date.ToString(), Start = "12:15", End = "13:00", Host = "Teknologkåren", Location = "Hänget", Language = "English", Capacity = 300};
            Events.AddRange(event1, event2, event3, event4, event5, event6, event7, event8);
            SaveChanges();

            // Tickets
            var ticket1 = new  Ticket { Id = -1,  Code = Guid.NewGuid(), PhotoOk = true, Event = event1, EventId = event1.Id.Value, UserId = user2.Id.Value , isConsumed = true};
            var ticket2 = new  Ticket { Id = -2,  Code = Guid.NewGuid(), PhotoOk = false, Event = event1, EventId = event1.Id.Value, UserId = user3.Id.Value };
            var ticket3 = new  Ticket { Id = -3,  Code = Guid.NewGuid(), PhotoOk = true, Event = event1, EventId = event1.Id.Value, UserId = user4.Id.Value };

            var ticket4 = new  Ticket { Id = -4,  Code = Guid.NewGuid(), PhotoOk = false, EventId = event2.Id.Value, UserId = user2.Id.Value };

            var ticket5  = new  Ticket { Id = -5, Code = Guid.NewGuid(), PhotoOk = true,  EventId = event3.Id.Value, UserId = user3.Id.Value };
            var ticket6  = new  Ticket { Id = -6, Code = Guid.NewGuid(), PhotoOk = true,  EventId = event4.Id.Value, UserId = user3.Id.Value };
            var ticket7  = new  Ticket { Id = -7, Code = Guid.NewGuid(), PhotoOk = true,  EventId = event4.Id.Value, UserId = user4.Id.Value };
            var ticket8  = new  Ticket { Id = -8, Code = Guid.NewGuid(), PhotoOk = true,  EventId = event5.Id.Value, UserId = user3.Id.Value };
            var ticket9  = new  Ticket { Id = -9, Code = Guid.NewGuid(), PhotoOk = true,  EventId = event6.Id.Value, UserId = user3.Id.Value };
            var ticket10 = new Ticket { Id = -10, Code = Guid.NewGuid(), PhotoOk = true,  EventId = event7.Id.Value, UserId = user8.Id.Value };
            var ticket11 = new Ticket { Id = -11, Code = Guid.NewGuid(), PhotoOk = true,  EventId = event6.Id.Value, UserId = user2.Id.Value };
            var ticket12 = new Ticket { Id = -12, Code = Guid.NewGuid(), PhotoOk = false, EventId = event8.Id.Value, UserId = user9.Id.Value };
            Tickets.AddRange(ticket1, ticket2, ticket3, ticket4, ticket5, ticket6, ticket7, ticket8, ticket9, ticket10, ticket11, ticket12); 
            SaveChanges();

            // StudentSessionTimeslots
            var session1 = new StudentSessionTimeslot { Id = -1, Start = DateTime.Parse("2021-11-21 10:00"), End = DateTime.Parse("2021-11-21 10:15"), Location = "Zoom", CompanyId = company1.Id.Value };
            var session2 = new StudentSessionTimeslot { Id = -2, Start = DateTime.Parse("2021-11-21 10:15"), End = DateTime.Parse("2021-11-21 10:30"), Location = "Zoom", CompanyId = company1.Id.Value };
            var session3 = new StudentSessionTimeslot { Id = -3, Start = DateTime.Parse("2021-11-21 10:30"), End = DateTime.Parse("2021-11-21 10:45"), Location = "Zoom", CompanyId = company1.Id.Value };

            var session4 = new StudentSessionTimeslot { Id = -4, Start = DateTime.Parse("2021-11-22 11:00"), End = DateTime.Parse("2021-11-22 11:15"), Location = "Zoom", CompanyId = company2.Id.Value };
            var session5 = new StudentSessionTimeslot { Id = -5, Start = DateTime.Parse("2021-11-22 11:15"), End = DateTime.Parse("2021-11-22 11:30"), Location = "Zoom", CompanyId = company2.Id.Value };

            var session6 = new StudentSessionTimeslot { Id = -6, Start = DateTime.Parse("2021-11-23 12:00"), End = DateTime.Parse("2021-11-22 12:15"), Location = "Zoom", CompanyId = company3.Id.Value };
            var session7 = new StudentSessionTimeslot { Id = -7, Start = DateTime.Parse("2021-11-23 12:15"), End = DateTime.Parse("2021-11-22 12:30"), Location = "Zoom", CompanyId = company3.Id.Value };
            StudentSessionTimeslots.AddRange(session1, session2, session3, session4, session5, session6, session7);
            SaveChanges();

            // StudentSessionApplications
            var application1 = new StudentSessionApplication { Id = -1, Motivation = "Hej, jag är jättebra och tror att ni vill träffa mig!", StudentId = student1.Id.Value, CompanyId = company1.Id.Value };
            var application2 = new StudentSessionApplication { Id = -2, Motivation = "I love my MacBook", StudentId = student2.Id.Value, CompanyId = company1.Id.Value };
            var application3 = new StudentSessionApplication { Id = -3, Motivation = "User experience is very important for me", StudentId = student3.Id.Value, CompanyId = company1.Id.Value };

            var application4 = new StudentSessionApplication { Id = -4, Motivation = "I would like to learn more about searching", StudentId = student1.Id.Value, CompanyId = company2.Id.Value };
            var application5 = new StudentSessionApplication { Id = -5, Motivation = "I am applying for everything", StudentId = student2.Id.Value, CompanyId = company2.Id.Value };
            var application6 = new StudentSessionApplication { Id = -6, Motivation = "Search algrorithms are very cool", StudentId = student3.Id.Value, CompanyId = company2.Id.Value };

            var application7 = new StudentSessionApplication { Id = -7, Motivation = "Music is a big passion of mine", StudentId = student2.Id.Value, CompanyId = company3.Id.Value };
            StudentSessionApplications.AddRange(application1, application2, application3, application4, application5, application6, application7);
            SaveChanges();

            // Contacts
            var contact1 = new Contact { Id = -1, FirstName = "PL", LastName = "Pappa", RoleInArkad = "Project Leader", Email = "contact1@example.com", PhoneNumber = "001-111 11 11" };
            var contact2 = new Contact { Id = -2, FirstName = "Head", LastName = "Van IT", RoleInArkad = "Head of IT", Email = "contact2@example.com", PhoneNumber = "002-222 22 22" };
            var contact3 = new Contact { Id = -3, FirstName = "Front", LastName = "End", RoleInArkad = "Frontend Manager", Email = "contact3@example.com", PhoneNumber = "003-333 33 33" };
            var contact4 = new Contact { Id = -4, FirstName = "Back", LastName = "End", RoleInArkad = "Backend Manager", Email = "contact4@example.com", PhoneNumber = "004-444 44 44" };
            Contacts.AddRange(contact1, contact2, contact3, contact4);
            SaveChanges();

            // FrequentAskedQuestion
            var faq1 = new FrequentAskedQuestion { Id = -1, Question = "Frequent Asked Question 1", Answer = "Gooooddd answerrr!!!"};
            var faq2 = new FrequentAskedQuestion { Id = -2, Question = "Frequent Asked Question 2", Answer = "Ye probably"};
            var faq3 = new FrequentAskedQuestion { Id = -3, Question = "Frequent Asked Question 3", Answer = "Hehe nop"};
            var faq4 = new FrequentAskedQuestion { Id = -4, Question = "Frequent Asked Question 4", Answer = "ChatGPT says: What is the meaning of life?"};
            FrequentAskedQuestion.AddRange(faq1, faq2, faq3, faq4);
            SaveChanges();

        }
    }
}