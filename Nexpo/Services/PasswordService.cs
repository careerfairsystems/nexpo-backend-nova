using BCryptNet = BCrypt.Net.BCrypt;

namespace Nexpo.Services
{
    public class PasswordService
    {

        public bool IsStrongPassword(string password)
        {
            return password.Length >= 8;
        }
        public bool ValidatePassword(string password, string hash)
        {
            return BCryptNet.Verify(password, hash);
        }

        public string HashPassword(string password)
        {
            return BCryptNet.HashPassword(password);
        }
    }
}
