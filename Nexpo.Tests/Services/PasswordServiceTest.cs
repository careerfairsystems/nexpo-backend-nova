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
        public void ValidatePassword_Should_Fail_Invalid_Password()
        {
            var password = "invalid_password";
            var passwordHash = "$2a$10$tEQ2YhuuMoHKHVMeAPQhf.C551s3u.TSYfaELIYtUyihbQ.RQOIjK";

            var isValid = _passwordService.ValidatePassword(password, passwordHash);

            Assert.False(isValid);
        }

        [Fact]
        public void ValidatePassword_Should_Fail_Empty_Password()
        {
            var emptyPassword = "";
            string nullPassword = null;
            var passwordHash = "$2a$10$tEQ2YhuuMoHKHVMeAPQhf.C551s3u.TSYfaELIYtUyihbQ.RQOIjK";

            var emptyIsValid = _passwordService.ValidatePassword(emptyPassword, passwordHash);
            var nullIsValid = _passwordService.ValidatePassword(nullPassword, passwordHash);

            Assert.False(emptyIsValid);
            Assert.False(nullIsValid);
        }

        [Fact]
        public void ValidatePassword_Should_Fail_Empty_Hash()
        {
            var password = "password";
            var emptyHash = "";
            string nullHash = null;

            var emptyIsValid = _passwordService.ValidatePassword(password, emptyHash);
            var nullIsValid = _passwordService.ValidatePassword(password, nullHash);

            Assert.False(emptyIsValid);
            Assert.False(nullIsValid);
        }
    }
}
