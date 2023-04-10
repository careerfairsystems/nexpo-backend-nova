using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Sustainsys.Saml2.AspNetCore2;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Saml2P;
using System;
using System.Linq;

namespace Nexpo.Controllers
{
    public class InitiateSSO : Controller
    {
        private readonly IConfiguration configuration;
        private readonly Saml2Options options;

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
        public IActionResult InitiateSingleSignOn(string returnUrl)
        {
            //The idp
            //...

            //Create a new SAML request
            var request = new Saml2AuthenticationRequest();
            
            var ACSUrl = new Uri(configuration["Saml2:AssertionConsumerServiceUrl"]);
            request.AssertionConsumerServiceUrl = ACSUrl;

            //add spoptions to request

            //Redirect to the idp
            
            
                
        }
    }
}

