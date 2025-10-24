using LAFABRICA.Data.DB;
using LAFABRICA.Services.Email;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace LAFABRICA.Services.Auth
{
    public class DemexAuthService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IConfiguration _config;
        private readonly EmailService _email;

        public DemexAuthService(IDbContextFactory<AppDbContext> context, IConfiguration config, EmailService email)
        {
            _contextFactory = context;
            _config = config;
            _email = email;
        }


        public async Task<User?> Login(string email, string password)
        {
            using var context = _contextFactory.CreateDbContext();

            var user = await context.Users
                .Include(u => u.Rol)
                .ThenInclude(r => r.RolePermissions)
                .FirstOrDefaultAsync(u =>
                    u.Email == email &&
                    u.IsActive == 1);
            if (user == null)
                throw new Exception("Usuario no encontrado");

            return BCrypt.Net.BCrypt.Verify(password, user.Password) ? user : null;

        }
        public async Task<User?> GetUserByEmail(string email)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Users
                .Include(u => u.Rol)
                .ThenInclude(r => r.RolePermissions)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ChangePassword( int userId, string currentPass, string newPass)
        {
            using var context = _contextFactory.CreateDbContext();

            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) 
                throw new Exception("Usuario no encontrado");
            if (!BCrypt.Net.BCrypt.Verify(currentPass, user.Password))
                    throw new Exception("La contraseña actual es incorrecta");

            if (!PasswordValidator.IsValid(newPass))
                throw new Exception("La contraseña debe tener al menos 8 caracteres, letras y números.");

            user.Password = user.Password = BCrypt.Net.BCrypt.HashPassword(newPass);

            context.Users.Update(user);
            await context.SaveChangesAsync();
            return true;    
            
        }

        public async Task GeneratePasswordResetAsync(string email)
        {
            using var context = _contextFactory.CreateDbContext();

            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email 
            && u.IsActive == 1);
            if (user == null)
                return; 

            var resetToken = await CreatePasswordResetTokenAsync(context, user.Id);

            var (subject, body) = BuildPasswordResetEmail(user.Name, resetToken.Token);

            await _email.sendEmail(user.Email, subject, body);
        }


        public async Task<bool> ResetPasswordAsync(string token, string newPass)
        {
            using var context = _contextFactory.CreateDbContext();

            var reset = await context.PasswordResetTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == token);

            if (reset == null || reset.Expiration < DateTime.UtcNow)
                throw new Exception("El enlace ha expirado o no es válido.");

            if (!PasswordValidator.IsValid(newPass))
                throw new Exception("La contraseña debe tener al menos 8 caracteres, letras y números.");

            reset.User.Password = BCrypt.Net.BCrypt.HashPassword(newPass);
            context.PasswordResetTokens.Remove(reset);

            await context.SaveChangesAsync();
            return true;
        }

        private async Task<PasswordResetToken> CreatePasswordResetTokenAsync(AppDbContext context, int userId)
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(32);
            var token = Convert.ToBase64String(tokenBytes);

            var reset = new PasswordResetToken
            {
                UserId = userId,
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(15)
            };

            context.PasswordResetTokens.Add(reset);
            await context.SaveChangesAsync();

            return reset;
        }


        private (string subject, string body) BuildPasswordResetEmail(string userName, string token)
        {
            var baseUrl = _config["AppSettings:BaseUrl"]?.TrimEnd('/');
            var url = $"{baseUrl}/reset-password/{Uri.EscapeDataString(token)}";

            var subject = "Restablecer contraseña - DEMEX";

            var body = $@"
                <h3>Hola {userName},</h3>
                <p>Has solicitado restablecer tu contraseña.</p>
                <p><a href=""{url}"" 
                    style=""display:inline-block;background-color:#007bff;color:white;
                           padding:10px 20px;border-radius:5px;text-decoration:none"">
                    Restablecer contraseña
                </a></p>
                <p>Este enlace expirará en 15 minutos.</p>
                <hr/>
                <small>Si no fuiste vos, podés ignorar este correo.</small>";

            return (subject, body);
        }

    }
}
