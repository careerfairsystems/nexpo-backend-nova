using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Sustainsys.Saml2;
using Sustainsys.Saml2.AspNetCore2;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nexpo.Services{
    public class SamlAuthService : IAuthenticationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Saml2Handler _saml2Handler;

        public SamlAuthService(IHttpContextAccessor httpContextAccessor, Saml2Handler saml2Handler)
        {
            _httpContextAccessor = httpContextAccessor;
            _saml2Handler = saml2Handler;
        }

        public Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string scheme)
        {
            
            return null;
        }

        public Task ChallengeAsync(HttpContext context, string scheme, AuthenticationProperties properties)
        {
            
            
            return Task.CompletedTask;
        }

        public Task ForbidAsync(HttpContext context, string scheme, AuthenticationProperties properties)
        {
            
            return Task.CompletedTask;
        }

        public Task SignInAsync(HttpContext context, string scheme, ClaimsPrincipal principal, AuthenticationProperties properties)
        {
            
            return Task.CompletedTask;
        }

        public Task SignOutAsync(HttpContext context, string scheme, AuthenticationProperties properties)
        {
            
            return Task.CompletedTask;
        }
    }
}