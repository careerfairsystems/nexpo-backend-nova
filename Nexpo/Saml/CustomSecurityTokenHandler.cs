using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace Nexpo.Saml{


    public class CustomSecurityTokenHandler : Sustainsys.Saml2.Saml2P.Saml2PSecurityTokenHandler
        {
            protected override ClaimsIdentity CreateClaimsIdentity(Saml2SecurityToken samlToken, string issuer, TokenValidationParameters validationParameters)
            {
                // Custom for SWAMID - add eduPersonPrincipalName, mail, givenName, sn
                var claimsIdentity = base.CreateClaimsIdentity(samlToken, issuer, validationParameters);
                var nameId = samlToken.Assertion.Subject.NameId;
                var nameIdClaim = new Claim(ClaimTypes.NameIdentifier, nameId.Value);
                claimsIdentity.AddClaim(nameIdClaim);
                var eduPersonPrincipalName = samlToken.Assertion.Statements.OfType<Saml2AttributeStatement>().FirstOrDefault()?.Attributes.FirstOrDefault(x => x.Name == "eduPersonPrincipalName")?.Values.FirstOrDefault();
                if (eduPersonPrincipalName != null)
                {
                    var eduPersonPrincipalNameClaim = new Claim(ClaimTypes.Name, eduPersonPrincipalName);
                    claimsIdentity.AddClaim(eduPersonPrincipalNameClaim);
                }
                var mail = samlToken.Assertion.Statements.OfType<Saml2AttributeStatement>().FirstOrDefault()?.Attributes.FirstOrDefault(x => x.Name == "mail")?.Values.FirstOrDefault();
                if (mail != null)
                {
                    var mailClaim = new Claim(ClaimTypes.Email, mail);
                    claimsIdentity.AddClaim(mailClaim);
                }
                var givenName = samlToken.Assertion.Statements.OfType<Saml2AttributeStatement>().FirstOrDefault()?.Attributes.FirstOrDefault(x => x.Name == "givenName")?.Values.FirstOrDefault();
                if (givenName != null)
                {
                    var givenNameClaim = new Claim(ClaimTypes.GivenName, givenName);
                    claimsIdentity.AddClaim(givenNameClaim);
                }
                var sn = samlToken.Assertion.Statements.OfType<Saml2AttributeStatement>().FirstOrDefault()?.Attributes.FirstOrDefault(x => x.Name == "sn")?.Values.FirstOrDefault();
                if (sn != null)
                {
                    var snClaim = new Claim(ClaimTypes.Surname, sn);
                    claimsIdentity.AddClaim(snClaim);
                }
                return claimsIdentity;
            }
        }
}