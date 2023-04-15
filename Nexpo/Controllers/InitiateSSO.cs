using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web;
using System.Xml;

namespace Nexpo.Controllers
{
    public class InitiateSSO : Controller
    {
        private readonly IConfiguration configuration;

        public InitiateSSO(IConfiguration configuration)
        {
            this.configuration = configuration;
            
        }


        
        /// <summary>
        /// The Single Sign-On (SSO) Service endpoint
        ///
        /// The endpoint that sends and SSO request to the IdP to authenticate the user
        /// </summary>
        [EnableCors]
        [AllowAnonymous]
        [HttpGet("InitiateSingleSignOn")]
        public IActionResult InitiateSingleSignOn()
        {
            // Create authentication properties to store the relay state, which is used to redirect the user back to the correct URL
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = "/", 
                // Redirect to the home page after the SSO process is complete
                // better to redirect them to a logged in page? are they authorized?
            };

            // Initiate the SAML authentication process for the user
            return Challenge(authenticationProperties, "Saml2");
            
            
            
                
        }
    }
}

