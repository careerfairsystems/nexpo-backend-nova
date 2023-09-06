using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nexpo.Models;
using Nexpo.Repositories;
using Nexpo.Services;
using Saml;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nexpo.Controllers
{
    [Route("api")]
    [ApiController]
    public class SAMLController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        protected readonly TokenService _tokenService;

        private readonly IStudentRepository _studentRepo;

        public SAMLController(
            IUserRepository iUserRepo,
            TokenService tokenService,
            IStudentRepository studentRepo
        )
        {
            _userRepo = iUserRepo;
            _tokenService = tokenService;
            _studentRepo = studentRepo;
        }

        /// <summary>
        /// Initiate a SAML login.
        /// Redirects the user to the SAML IDP.
        /// </summary>
        [HttpGet]
        [Route("saml/InitiateSingleSignOn")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult InitiateSingleSignOn()
        {
            //The SAML provider url, aka "Endpoint"
            var samlEndpoint = "https://idpv4.lu.se/idp/profile/SAML2/Redirect/SSO";

            var request = new AuthRequest(
                "https://www.nexpo.arkadtlth.se/api/saml/SP", //our app's "entity ID" here
                "https://www.nexpo.arkadtlth.se/api/Saml/ACS" //our Assertion Consumer URL
            );

            return Redirect(request.GetRedirectUrl(samlEndpoint));
        }

        /// <summary>
        /// Assertion Consumer Service.
        /// Where the IDP should redirect users after authenticating.
        /// </summary>
        [HttpPost]
        [Route("Saml/ACS")]
        public async Task<IActionResult> ACS()
        {
            Console.WriteLine("...");
            // The certificate of our SAML idp
            string samlCertificate =
            @"
            MIIEGDCCAoCgAwIBAgIVAKdIP7eMddg3zDrWrc+09WAj2x7AMA0GCSqGSIb3DQEB
            CwUAMBYxFDASBgNVBAMMC2lkcHY0Lmx1LnNlMB4XDTIwMDQyMjEyNTgzMloXDTQw
            MDQyMjEyNTgzMlowFjEUMBIGA1UEAwwLaWRwdjQubHUuc2UwggGiMA0GCSqGSIb3
            DQEBAQUAA4IBjwAwggGKAoIBgQCvHmABYJf9A/WJ5+cUcWp9dTSfMZQZHJ+z7B04
            J+H133U7VP7jBHl36XL2CEdl7G0eCbk01Cw7IbkcIaQwLa3dJGgADMM8Lh0iE/BE
            4YG1p779QdZU77ZT/91FDsXn/B9qsY0NsYxWCvxUN52qX7+a9ZgW35xEfMDEF6ZW
            R3Bll3VZyqwtYbE7jU/f0ujyRFmntYl3dGMRamlzu9o7+sXLi1xlceJMWWADh63O
            eNGWm2XKWeGtmGyyyeo8mQbDBZNmS6K5WEIznhzXPJfbwMlXrLye041/r2kppB+Y
            4nISfL6S1IuPObw0HM9kosBlrwZFjP++Wodmt6OHXiHLbKA2rj/Jxt7BpxIGM5It
            7cEFAn6J/5MvsqGmWVLnCMPF1vzq866oQdDLfNJXCbyOgvPDPnw/Zl6bb3qYx5Kk
            PvQsTMK36e8zB/pFrzagSgbFkYQuUA+KNBgIpgeMO3bpoRZ4S7wc0Vq7JShJ9fBQ
            uP6NtJw6iTehHMDFTn8kbU9LxnECAwEAAaNdMFswHQYDVR0OBBYEFMUWZ/DnrU5S
            unY+y/Upo+dVljnpMDoGA1UdEQQzMDGCC2lkcHY0Lmx1LnNlhiJodHRwczovL2lk
            cHY0Lmx1LnNlL2lkcC9zaGliYm9sZXRoMA0GCSqGSIb3DQEBCwUAA4IBgQALwnh8
            uEl9xWrin4vPLm+Mc0THPPVaeGZF1ivyDcY5WefOXaxaX80BxHKOcA2aG/+Ne/Ko
            k9u1COHrjp3QfkHE2SQTAPoD4EWtHaiCyoBYnwRA1qfSFbcnVlZOr9IOIRAA6TiS
            iE+G4kf9QW/xUSmONPGLU8vqXuisxTr46XjQMbl+dtTZ5fxURKFFv21C0KBXMkLI
            xSzTTn9Q0acVT07oNgNQscJtWtStXbsrEhQ9+uyEd3xCF883BjpF4nDDtLOo0jXD
            iyhJcGU4fKD4MMeREoKb+OKLpCqE+8BdmO93kMJLrbdS1EPIlVG1fShtJCeF40EL
            C4Ns9OOYE0uD7tQG4oQQht3WFSiS95Plylg62BGqk5LpwODuDpuO7tqZkLbI5gN1
            7S98DoMZftlEg8leJv1NuaBIKD/C/WI6OPAqVmBCckb6R8eGU7yGGc2i2qZ9yC2r
            3SMS6VsKOZJv77OE9yzH7JF8aKjaSNHu1lpP42IG/hmrAgNjQMTbXFyni5o=";

            // Read the data - SAML providers usually POST it into the "SAMLResponse" var
            var samlResponse = new Response(samlCertificate, Request.Form["SAMLResponse"]);

            if (samlResponse.IsValid())
            {

                var email = samlResponse.GetEmail();
                var firstname = samlResponse.GetFirstName();
                var lastname = samlResponse.GetLastName();

                var user = await _userRepo.FindByEmail(email.ToLower());

                if (user == null)
                {
                    user = new User
                    {
                        Role = Role.Student,
                        Email = email,
                        FirstName = firstname,
                        LastName = lastname,
                    };

                    await _userRepo.Add(user);

                    var _student = new Student
                    {
                        UserId = user.Id.Value
                    };

                    await _studentRepo.Add(_student);

                }

                var claims = new List<Claim>
                {
                    new Claim(UserClaims.Id, user.Id.ToString()),
                    new Claim(UserClaims.Role, user.Role.ToString()),
                };

                if (user.Role == Role.Student)
                {
                    var student = await _studentRepo.FindByUser(user.Id.Value);
                    claims.Add(new Claim(UserClaims.StudentId, student.Id.ToString()));
                }

                if (user.Role == Role.Volunteer)
                {
                    var volunteer = await _userRepo.Get(user.Id.Value);
                    claims.Add(new Claim(UserClaims.VolunteerId, volunteer.Id.ToString()));
                }

                var jwt = _tokenService.GenerateJWT(claims);

                return Redirect("https://www.nexpo.arkadtlth.se/api/saml/" + jwt);
            }

            return Content("Unauthorized");
        }

        /// <summary>
        /// Currently just a placeholder to hold the JWT,
        /// so it can be collected by the frontend.
        /// However, can be used to instatly replace the JWT with a new,
        /// that has not been exposed to the used (may cause security issues?)
        /// </summary>
        [HttpGet]
        [Route("saml/{jwt}")]
        public async Task JWT(){
            
        }

        [HttpGet]
        [Route("saml/Logout")]
        public async Task<IActionResult> Logout()
        {

            return Redirect("~/");
        }

        /// <summary>
        /// Mostly exists to visualize the entity ID as a endpoint. 
        /// The certificate of us (the service provider)
        /// </summary>
        [HttpGet]
        [Route("saml/SP")]
        public String SP()
        {
            // The certificate of us (the service provider)
            return @"NOT AVAILABLE ON GITHUB";


        }
    }

}