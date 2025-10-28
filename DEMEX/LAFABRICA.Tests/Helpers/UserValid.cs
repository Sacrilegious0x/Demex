using LAFABRICA.Data.DB;

namespace LAFABRICA.Tests.Helpers
{
    internal static class UserValid
    {
        public static User CreateValidUser(int rolId, string Identification = "707770777",
            string email = "userValid@gmail.com")
        {
            return new User
            {
                Identification = Identification,
                Name = "Gabs",
                Email = email,
                Password = "Pass123!",
                Speciality = "Tester",
                IsActive = 1,
                RolId = rolId,
                UserType = "Admin"
            };
        }
    }
}
