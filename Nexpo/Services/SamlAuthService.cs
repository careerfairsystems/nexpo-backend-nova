using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Sustainsys.Saml2;
using Sustainsys.Saml2.AspNetCore2;
using System;
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

        public async Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string scheme)
        {
            /* // Check if the requested authentication scheme is supported
            if (string.IsNullOrEmpty(scheme) || !scheme.Equals("Saml2", StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.Fail("Unsupported authentication scheme");
            } */
            
            // Attempt to extract the SAML response from the HTTP request
            var saml2Auth = await context.AuthenticateAsync("Saml2");
            //if (!saml2Auth.Succeeded)
            //{
            //    return AuthenticateResult.Fail("SAML authentication failed");
            //}
        
            // Retrieve the claims from the SAML response
            var claims = saml2Auth.Principal.Claims;
        
            // Create a new ClaimsIdentity containing the retrieved claims
            var identity = new ClaimsIdentity(claims, scheme);
        
            // Create a new ClaimsPrincipal containing the retrieved identity
            var principal = new ClaimsPrincipal(identity);
        
            // Return a successful authentication result containing the created principal
            return AuthenticateResult.Success(new AuthenticationTicket(principal, scheme));
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