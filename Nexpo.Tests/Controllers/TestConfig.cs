using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nexpo.Controllers;
using Nexpo.Models;
using Nexpo.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace Nexpo.Tests.Services
{
    public class TestConfig
    {
        private readonly TokenService _tokenService;
        private readonly UsersController _usersController;

        public TestConfig()
        {
            PasswordService passwordService = new PasswordService();
            DbContextOptions<ApplicationDbContext> dbContextOptions = new DbContextOptions<ApplicationDbContext>();
            ApplicationDbContext applicationDbContext = new ApplicationDbContext(dbContextOptions, passwordService);
            applicationDbContext.Seed();


        }
    }
}
