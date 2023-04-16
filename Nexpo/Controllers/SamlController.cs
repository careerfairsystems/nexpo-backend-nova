using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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

        /// <summary>
        /// SSO SingleLogoutService (SLO) endpoint
        ///
        /// The endpoint the Identity Provider (IdP) sends logout requests to,
        /// in order to terminate the user's session.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            //await HttpContext.SignOutAsync(Saml2Defaults.Scheme);

            return this.Ok();
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

        /// <summary>
        /// The endpoint corresponding to the entityID in the metadata
        /// 
        /// Plans to be used to generate metadata
        /// </summary>
        [HttpGet("SP")]
        public Task<IActionResult> SP()
        {
            var metadataPath = "../../metadata.xml";
            var xmlContent = System.IO.File.ReadAllText(metadataPath);
            return Task.FromResult<IActionResult>(Content(xmlContent, "text/xml"));

            
            
        }





    }

}
