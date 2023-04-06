using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Sustainsys.Saml2.AspNetCore2;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Nexpo.Constants;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Options;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Metadata;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace Nexpo.Controllers
{
    [Route("api/Saml")]
    public class SamlController : Controller
    {
        private readonly IConfiguration configuration;

        public SamlController(IConfiguration configuration)
        {
            this.configuration = configuration;

        }
        
	    [EnableCors]
        [AllowAnonymous]
        [HttpGet("InitiateSingleSignOn")]
        public IActionResult InitiateSingleSignOn(string returnUrl)
        {
	        // pretty ugly should be fixed later
	        returnUrl = "https://www.nexpo.arkadtlth.se/api/saml/Callback/"; 
            return new ChallengeResult(
                Saml2Defaults.Scheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action(nameof(LoginCallback), new { returnUrl })
                }
            );
        }

        [HttpGet("Metadata")]
        public async Task<IActionResult> Metadata()
        {   
            var saml2Options = new Saml2Options();
            configuration.GetSection("Saml2").Bind(saml2Options); //rätt section?

            var SPOptions = saml2Options.SPOptions;

            var entityDescriptor = new EntityDescriptor();
            entityDescriptor.EntityId = SPOptions.EntityId;

            var sp = new SpSsoDescriptor();

            var ACS = new AssertionConsumerService
            {
                Binding = new Uri("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST"),
                Location = new Uri("https://www.nexpo.arkadtlth.se/api/saml/ACS"),
                Index = 0,
            };

            sp.AssertionConsumerServices.TryAdd(0, ACS);

            var SLO = new SingleLogoutService
            {
                Binding = new Uri("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-REDIRECT"),
                Location = new Uri("https://www.nexpo.arkadtlth.se/api/saml/Logout"),
            };

            sp.SingleLogoutServices.Add(SLO);

            var technicalContactMail = "it.arkad@tlth.se";
            var SPX509cert = SPOptions.ServiceCertificates[0];

            var nameIdFormat = new Uri("urn:oasis:names:tc:SAML:2.0:nameid-format:persistent"); //rätt?

            var callback = new Uri("https://www.nexpo.arkadtlth.se/api/saml/Callback"); 

            //NameId Format - kolla om rätt 
            //private key
            //WantAssertionsSigned
            //callback - lägg till
            

            


            


            //var idpOptions = saml2Options.IdentityProviders.Default;
            //var idpURi = new Uri(idpOptions.MetadataLocation);

            //var nameidformat = 

            //_______________________


            //var metadataSerializer = new MetadataSerializer();
            //var metadata = metadataSerializer.
            //return Content(metadata, "text/xml");
            
            /*
            plan: Lägg till saml2configuration samt mina endpoints i en xml fil, och displaya den.
            
            Måste byta lite localhost endpoitns

            Hur hämtar jag mina endpoints? Double as localhost and onserver
            
            */


        }

        
        [AllowAnonymous]
        [HttpGet("Callback")]
        public async Task<IActionResult> LoginCallback(string returnUrl)
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(ApplicationSamlConstants.External);

            if (!authenticateResult.Succeeded)
            {
                return Unauthorized();
            }

            var token = this.CreateJwtSecurityToken(authenticateResult);
            HttpContext.Session.SetString("JWT", new JwtSecurityTokenHandler().WriteToken(token));

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);

            }

            return this.Ok();
        }

        //ACS is an abbreviation for AttributeConsumerService
        [AllowAnonymous]
        [HttpPost("ACS")]
        public async Task<IActionResult> AttributeConsumerService()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(ApplicationSamlConstants.External);

            if (!authenticateResult.Succeeded)
            {
                return Unauthorized();
            }

            var token = this.CreateJwtSecurityToken(authenticateResult);
            HttpContext.Session.SetString("JWT", new JwtSecurityTokenHandler().WriteToken(token));

            return this.Ok(); 
        }

        [AllowAnonymous]
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(Saml2Defaults.Scheme);

            return this.Ok();
        }

        private JwtSecurityToken CreateJwtSecurityToken(AuthenticateResult authenticateResult)
        {
            var claimsIdentity = new ClaimsIdentity(ApplicationSamlConstants.Application);
            claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier));

            var username = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier).Value.ToString();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, username),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["JWT:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                this.configuration["JWT:Issuer"],
                this.configuration["JWT:Issuer"],
                claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);
        }

    



    }

}
