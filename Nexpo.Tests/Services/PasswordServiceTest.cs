using Nexpo.Services;
using Xunit;

namespace Nexpo.Tests.Services
{
    public class PasswordServiceTest
    {
        private readonly PasswordService _passwordService;

        public PasswordServiceTest()
        {
            _passwordService = new PasswordService();
        }

        [Fact]
        public void HashPassword_Should_Not_Return_Input()
        {
            var password = "password";

            var passwordHash = _passwordService.HashPassword(password);

            Assert.NotEqual(passwordHash, password);
        }

        [Fact]
        public void ValidatePassword_Should_Validate_Valid_Password()
        {
            var password = "password";
            var passwordHash = "$2a$10$tEQ2YhuuMoHKHVMeAPQhf.C551s3u.TSYfaELIYtUyihbQ.RQOIjK";

            var isValid = _passwordService.ValidatePassword(password, passwordHash);

            Assert.True(isValid);
        }

        [Fact]
        public void ValidatePAssword_Should_Not_Accept_Invalid_Password()
        {
            var password = "invalid_password";
            var passwordHash = "$2a$10$tEQ2YhuuMoHKHVMeAPQhf.C551s3u.TSYfaELIYtUyihbQ.RQOIjK";

            var isValid = _passwordService.ValidatePassword(password, passwordHash);

            Assert.False(isValid);
        }
    }
}
