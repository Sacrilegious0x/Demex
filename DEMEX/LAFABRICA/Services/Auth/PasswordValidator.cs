using System.Text.RegularExpressions;

namespace LAFABRICA.Services.Auth
{
    public class PasswordValidator
    {
        public static bool IsValid(string password)
        {
            var pass = password.Trim();
            var regex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$");
            return regex.IsMatch(pass);

        }
    }
}
