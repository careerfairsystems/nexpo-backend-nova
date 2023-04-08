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
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Metadata;
using System.Xml;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;

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

        /// <summary>
        /// the endpoint corresponding to the entityID in the metadata
        /// does nothing currently but planning to use it to display metadata
        /// </summary>
        [HttpGet("SP")]
        public Task<IActionResult> SP()
        {
        
           var saml2Options = new Saml2Options();
            configuration.GetSection("Saml2").Bind(saml2Options); //rätt section?
            var SPOptions = saml2Options.SPOptions;
            //var serviceProvider = (ServiceProvider) Nexpo.Startup.ServiceProvider;


            //config entityID
            var entityDescriptor = new EntityDescriptor(SPOptions.EntityId);


            var SPX509cert = SPOptions.ServiceCertificates[0];
            var SPX509Data = new KeyInfoX509Data();
            SPX509Data.AddCertificate(SPX509cert.Certificate);
            var keyInfo = new DSigKeyInfo();
            //maybe add SPX509Data to keyInfo


            var SPX509Descriptor = new KeyDescriptor
            {
                Use = KeyType.Signing,
                KeyInfo = keyInfo,
            };


            //entityDescriptor.keyDiscriptors.Add(SPX509Descriptor);

            //config endpoints
            var spd = new SpSsoDescriptor();
            var ACS = new AssertionConsumerService
            {
                Binding = new Uri("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST"),
                Location = new Uri("https://www.nexpo.arkadtlth.se/api/saml/ACS"), //fullt att hårdkoda - refactor
                Index = 0,
            };


            var SLO = new SingleLogoutService
            {
                Binding = new Uri("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect"),
                Location = new Uri("https://www.nexpo.arkadtlth.se/api/saml/Logout"),
            };


            spd.AssertionConsumerServices.TryAdd(0, ACS);
            spd.SingleLogoutServices.Add(SLO);
            spd.WantAssertionsSigned = SPOptions.WantAssertionsSigned;
            entityDescriptor.RoleDescriptors.Add(spd);





            var callback = new Uri("https://www.nexpo.arkadtlth.se/api/saml/Callback");




            //var technicalContactMail = "it.arkad@tlth.se"; //lägg i config/constants   
            //var tokenHandler = SPOptions.Saml2PSecurityTokenHandler;
            //
            //var nameIdFormat = new Uri("urn:oasis:names:tc:SAML:2.0:nameid-format:persistent"); //rätt?







            var xmlDoc = new XmlDocument();
            var xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(xmlDeclaration);

            var entityDescriptorXMLChild = xmlDoc.CreateElement(entityDescriptor.Extensions.ToString());
            xmlDoc.AppendChild(entityDescriptorXMLChild);
            //NameId Format - kolla om rätt n
            //private key
            //WantAssertionsSigned
            //callback - lägg till
            //baseURL


            //kolla på add ta all info från SPOptions och lägg till i xml


            //outer?
            //return Content(xmlDoc.InnerText, "text/xml");



            return null;
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
