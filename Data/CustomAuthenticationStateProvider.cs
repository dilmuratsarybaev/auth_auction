using Example.Data.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace Example.Data
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {

        private readonly ProtectedSessionStorage _sessionStorage;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        public CustomAuthenticationStateProvider( ProtectedSessionStorage sessionStorage) {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userSessionStorageResult = await _sessionStorage.GetAsync<UserSession>("UserSession");
                var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;
                if (userSession == null)
                    return await Task.FromResult(new AuthenticationState(_anonymous));

                var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userSession.Name),
                new Claim(ClaimTypes.Email, userSession.Email),
                new Claim(ClaimTypes.Role, userSession.Role),
                new Claim("password", userSession.Password),
              
            };

                

                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(authClaims, "CustomAuth"));

                return await Task.FromResult(new AuthenticationState(claimsPrincipal));
            }
            catch
            {
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }
        }

        public async Task UpdateAuthenticationState(UserSession userSession)
        {
            ClaimsPrincipal claimsPrincipal;

            if (userSession != null)
            {
                await _sessionStorage.SetAsync("UserSession", userSession);


                var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userSession.Name),
                new Claim(ClaimTypes.Email, userSession.Email),
                new Claim(ClaimTypes.Role, userSession.Role),
                new Claim("password", userSession.Password),

            };

               
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(authClaims, "CustomAuth"));
            }
            else
            {
                await _sessionStorage.DeleteAsync("UserSession");
                claimsPrincipal = _anonymous;
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

    }
}
