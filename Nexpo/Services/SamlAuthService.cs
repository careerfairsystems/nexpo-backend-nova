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

        /// <summary>
        /// Initialize the SAML2 authentication process
        /// </summary>
        /// <param name="scheme">The authentication scheme that should be used</param>
        public async Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string scheme)
        {
            try
            {
                // Call the AuthenticateAsync method to authenticate the user
                var saml2Auth = await context.AuthenticateAsync(scheme);

                // Check if the authentication result is successful
                if (saml2Auth.Succeeded)
                {
                    return saml2Auth;
                }

                // TODO Handle authentication failures appropriately

                return AuthenticateResult.Fail("Authentication failed.");
            }
            catch (Exception ex)
            {
                // Log any errors that occur
                Console.Error.WriteLine($"An error occurred during SAML2 authentication: {ex}");
                throw;
            }
        }

        /// <summary>
        /// This method is called when the user is not authenticated.
        /// It initiates the SAML2 authentication process.
        /// </summary>
        /// <param name="scheme">The authentication scheme that should be used</param>
        /// <param name="properties">Dictionary used to store state values about the authentication session</param>
        public async Task ChallengeAsync(HttpContext context, string scheme, AuthenticationProperties properties)
        {
            try
            {
                // Call the ChallengeAsync method to initiate the SAML2 authentication process
                await context.ChallengeAsync(scheme, properties);
            }
            catch (Exception ex)
            {
                // Log any errors that occur
                Console.Error.WriteLine($"An error occurred during SAML2 authentication: {ex}");
                throw;
            }
        }

        /// <summary>
        /// This method is called when the user is not authorized to access a resource.
        /// </summary>
        /// <param name="scheme">The authentication scheme that should be used</param>
        /// <param name="properties">Dictionary used to store state values about the authentication session</param>
        public Task ForbidAsync(HttpContext context, string scheme, AuthenticationProperties properties)
        {
            Console.WriteLine("you are not permitted to access this resource");
            var saml2Forbid = context.ForbidAsync(scheme, properties);
            return saml2Forbid;
        }

        /// <summary>
        /// Sign a principal in for the specified authentication scheme.
        /// </summary>
        /// <param name="scheme">The authentication scheme that should be used</param>
        /// <param name="principal">A principal user (that has certain permission) within the application. 
        ///                         It's claims are pieces of information that are associated with a user's identity.</param>
        /// <param name="properties">Dictionary used to store state values about the authentication session</param>
        public Task SignInAsync(HttpContext context, string scheme, ClaimsPrincipal principal, AuthenticationProperties properties)
        {
            Console.WriteLine("c");
            var saml2SignIn = context.SignInAsync(scheme, principal, properties);
            return saml2SignIn;
        }

        /// <summary>
        /// Sign a principal out for the specified authentication scheme.
        /// </summary>
        /// <param name="scheme">The authentication scheme that should be used</param>
        /// <param name="properties">Dictionary used to store state values about the authentication session</param>
        public Task SignOutAsync(HttpContext context, string scheme, AuthenticationProperties properties)
        {
            Console.WriteLine("d");
            var saml2SignOut = context.SignOutAsync(scheme, properties);
            return saml2SignOut;
        }
    }
}