using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nexpo.Controllers;
using Nexpo.Models;
using Nexpo.Repositories;
using Nexpo.Services;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Sdk;

namespace Nexpo.Tests.Services
{
    public class TestConfig
    {
        private readonly UsersController _usersController;

        public TestConfig()
        {
            PasswordService passwordService = new PasswordService();
            DbContextOptions<ApplicationDbContext> dbContextOptions = new DbContextOptions<ApplicationDbContext>();
            ApplicationDbContext applicationDbContext = new ApplicationDbContext(dbContextOptions, passwordService);
            applicationDbContext.Seed();
            IUserRepository userRepository = new UserRepository(applicationDbContext);
            ICompanyConnectionRepository companyConnectionRepository = new CompanyConnectionRepository(applicationDbContext);
            IStudentSessionApplicationRepository studentSessionApplicationRepository = new StudentSessionApplicationRepository(applicationDbContext);

            _usersController = new UsersController(userRepository, companyConnectionRepository, studentSessionApplicationRepository,passwordService);

        }

        [Fact]
        public async void TestTest()
        {
            //TODO
            //No database provider has been configured for this DbContext
            Assert.Equal(null, await _usersController.GetUsers());
        }
    }
}
