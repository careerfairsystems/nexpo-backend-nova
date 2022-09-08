using Nexpo.Controllers;
using Nexpo.Repositories;
using Nexpo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Nexpo.Tests.Controllers
{
    public class UsersControllerTest : IClassFixture<TestDatabaseFixture>
    {
        UsersController _usersController;
        public UsersControllerTest(TestDatabaseFixture fixture)
        {
            Fixture = fixture;
            var context = Fixture.CreateContext();
            PasswordService _passwordService = new PasswordService();
            IUserRepository _userRepository = new UserRepository(context);
            ICompanyConnectionRepository _companyConnectionRepository = new CompanyConnectionRepository(context);
            IStudentSessionApplicationRepository _studentSessionApplicationRepository = new StudentSessionApplicationRepository(context);
            _usersController =
                new UsersController(_userRepository,
                _companyConnectionRepository,
                _studentSessionApplicationRepository,
                _passwordService);
        }
        public TestDatabaseFixture Fixture { get; }


        [Fact]
        public void TestGetUsers()
        {
            var users = _usersController.GetUsers();
            Assert.Equal("student1", users.ToString());
        }
    }
}
