using LAFABRICA.Data.DB;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using LAFABRICA.Models;

namespace LAFABRICA.Services.Auth
{
    public class DemexAuthenticationStateProvider : AuthenticationStateProvider
    {
        private  ClaimsPrincipal _principal = new ClaimsPrincipal(new ClaimsIdentity());
        private readonly DemexAuthService _service;
        private readonly ProtectedSessionStorage _sessionStorage;
        public DemexAuthenticationStateProvider(DemexAuthService service, ProtectedSessionStorage sessionStorage) {
            
            _service = service;
            _sessionStorage = sessionStorage;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
              

                var userActive = await _sessionStorage.GetAsync<string>("userEmail");
                if (userActive.Success && !string.IsNullOrEmpty(userActive.Value))
                {
                    var user = await _service.GetUserByEmail(userActive.Value);
                    if (user != null && user.Rol != null)
                    {
                        _principal = CreateClaimsPrincipal(user);
                        return new AuthenticationState(_principal);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ha ocurrio un problema al " +
                    $"intentar iniciar sesion con el usuario: {ex.Message}");
            }

            _principal = new ClaimsPrincipal(new ClaimsIdentity());
            return new AuthenticationState(_principal);
        }

        public async Task<bool> LoginAsync(string email, string password)
        {

            var user = await _service.Login(email, password);
            if (user != null && user.Rol != null)
            {
                _principal = CreateClaimsPrincipal(user);
                await _sessionStorage.SetAsync("userEmail", user.Email);

                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_principal)));
                return true;
            }

            await _sessionStorage.DeleteAsync("userEmail");
            _principal = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_principal)));

            return false;
        }


        public async Task LogoutAsync()
        {
            await _sessionStorage.DeleteAsync("userEmail");
            _principal = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_principal)));
        }

        private ClaimsPrincipal CreateClaimsPrincipal(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Rol?.Name ?? string.Empty),
                new Claim("UserType", user.UserType ?? string.Empty),
                new Claim("UserId", user.Id.ToString())
            };

            foreach (var permission in user.Rol!.RolePermissions)
            {
                claims.Add(new Claim($"can_{permission.Module}_{DemexClaims.View}", permission.Canview.ToString()));
                claims.Add(new Claim($"can_{permission.Module}_{DemexClaims.Create}",permission.Cancreate.ToString()));
                claims.Add(new Claim($"can_{permission.Module}_{DemexClaims.Edit}", permission.Canedit.ToString()));
                claims.Add(new Claim($"can_{permission.Module}_{DemexClaims.Delete}", permission.Candelete.ToString()));
            }

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "DemexAuth"));
        }

    }
}
