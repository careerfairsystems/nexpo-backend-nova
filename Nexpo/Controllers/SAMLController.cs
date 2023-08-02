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
    [Route("api/[controller]")]
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

        [HttpGet("InitiateSingleSignOn")]
        public IActionResult InitiateSingleSignOn()
        {
            //The SAML provider url, aka "Endpoint"
            var samlEndpoint = "https://idpv4.lu.se/idp/profile/SAML2/Redirect/SSO";

            var request = new AuthRequest(
                "https://www.nexpo.arkadtlth.se/api/Saml/SP", //our app's "entity ID" here
                "https://www.nexpo.arkadtlth.se/api/Saml/ACS" //our Assertion Consumer URL
            );

            return Redirect(request.GetRedirectUrl(samlEndpoint));
        }

        // Where the provider should redirect users after authenticating
        [HttpPost("ACS")]
        public async Task<IActionResult> ACS()
        {
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

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = null, 
                    IsPersistent = false,
                    AllowRefresh = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return Redirect("https://www.nexpo.arkadtlth.se/api/companies");
            }

            return Content("Unauthorized");
        }




        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("~/");
        }


        [HttpGet("SP")]
        public String SP()
        {
            // The certificate of us (the service provider)
            return @"
            MIIDozCCAosCFH1wAolC0dp70/NUOpUCzeeYMWNNMA0GCSqGSIb3DQEBCwUAMIGN
            MQswCQYDVQQGEwJTRTEPMA0GA1UECAwGU2NhbmlhMQ0wCwYDVQQHDARMdW5kMQ4w
            DAYDVQQKDAVBUktBRDELMAkGA1UECwwCSVQxFzAVBgNVBAMMDkJhY2tlbmRNYW5h
            Z2VyMSgwJgYJKoZIhvcNAQkBFhliYWNrZW5kLmFya2FkQGJveC50bHRoLnNlMB4X
            DTIzMDQwNzIzNTY1OVoXDTI0MDQwNjIzNTY1OVowgY0xCzAJBgNVBAYTAlNFMQ8w
            DQYDVQQIDAZTY2FuaWExDTALBgNVBAcMBEx1bmQxDjAMBgNVBAoMBUFSS0FEMQsw
            CQYDVQQLDAJJVDEXMBUGA1UEAwwOQmFja2VuZE1hbmFnZXIxKDAmBgkqhkiG9w0B
            CQEWGWJhY2tlbmQuYXJrYWRAYm94LnRsdGguc2UwggEiMA0GCSqGSIb3DQEBAQUA
            A4IBDwAwggEKAoIBAQDVIpys4RySPfjnlbnU2IQzDRy5rMIbhhqztt47W0udkoNa
            TTNDyKnWq7jPw3fuwzglbqFEN+VQhOP79IBnilO8XnkcIPERj/Rfvc6f4/hAnQUR
            keTw/XHkj1YNoYbQ0YabHDW8EdEGQ/nlfUkN0+JAoU6tXGYqDpnxcxPcIhay3pgo
            pBO2vr+XDpD1tmc2L18Y4PoLduOE1DpXhRr/qWf0il/zlcSqiU212nWFNc0g6mGM
            NdfmADgA8e+R0SCwfUblJRofLisX534mhyhrHjV/XzGQnMzQXAGhIwC5izndUWtw
            Bs/sKcOWjDUu8+B8vzCLRKJu0gK1yheRywXsCfTNAgMBAAEwDQYJKoZIhvcNAQEL
            BQADggEBAJdkiZn/Ps+uDF743IsJr7HHTG904BzyZDFkkdW313QP/dBwpGfpGxxl
            vnpHlqNBxvB1+nRNcxN7ZQ1oPvl2sq1xAcqIYeLm9zQBL4Qy68R7rIhKnIdNi4l6
            qAsLHryLeBhlU+Gkc9bgMtgp5zegNuNBLsTGZ4+zQFN/iNoUtK1q5gct4aoOKZMT
            7/5OKGgXnyI6wf+4VNU/w9L9GPSzK9DgfTst+B4sllubAFdUMK4cfUYa5ueNo0zr
            1YP3bxLPE21hOPXW0UA82rFpXm9E+HNSIjARLTPRLrUGStb3+t3a+wYplGpCr4aD
            f4WQ/6968/Njivn1i+eAKEwSP004hPc=";


        }
    }

}