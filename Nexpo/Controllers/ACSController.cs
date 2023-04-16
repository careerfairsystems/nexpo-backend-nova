using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;

namespace Nexpo.Controllers
{
    [Route("api/Saml")]
    public class AssertionConsumerService : Controller
    {
        public AssertionConsumerService() { }

        /// <summary>
        /// SSO AssertionConsumerService endpoint
        ///
        /// The endpoint that the IdP will post the SAMLResponse to
        /// Meaning after a successful login, the idp performs a redirect to this endpoint
        /// The HttpContext will contain the SAMLResponse
        /// ACS is an abbreviation for AttributeConsumerService
        /// </summary>
        [AllowAnonymous]
        [HttpPost("ACSLegacy")]
        [Authorize(AuthenticationSchemes = "Saml2")] //Forced to user to be authenticated with Saml2 before this endpoint is reached (tror detta ska göras)
        public void AttributeConsumerService()
        {
            var user = HttpContext.User;

            XmlDocument samlXML = new XmlDocument();

            // Get SAMLResponse from POST
            var response = HttpContext.Request.Form["SAMLResponse"].ToString();
            var responseBytes = Convert.FromBase64String(response);
            String SAMLResponseString = System.Text.Encoding.UTF8.GetString(responseBytes);
            samlXML.LoadXml(SAMLResponseString);

            // Validate X509 Certificate Signature
            if (!ValidateX509CertificateSignature(samlXML))
            {
                //invalid signature - handle that
                //context.Response.Redirect("ServiceProviderError.aspx"); något i den stilen?
            }

            // Extract SSO Data from SAMLResponse
            AssertionData SSOData = new AssertionData(samlXML);

            // TODO: Still need to do something with the information

            //Is this redirect correct? - nvm I want to log them in. Redo this
            HttpContext.Response.Redirect(HttpContext.Request.Form["RelayState"].ToString());
        }

        /// <summary>
        /// Data structure for the parsed SAML Assertion 
        /// </summary>
        private class AssertionData
        {
            public Dictionary<string, string> SAMLAttributes;

            public AssertionData(XmlDocument samlXML)
            {
                //Currently only extracts the name id. Might want to extract more information
                SAMLAttributes = new Dictionary<string, string>();
                var nameIDElement = samlXML.GetElementsByTagName("NameID")[0];
                string nameID = nameIDElement.InnerText;
                SAMLAttributes.Add("NameID", nameID);
            }
        }

        /// <summary>
        /// Validates the X509 Certificate Signature of the SAMLResponse
        /// Meaning checks that the SAMLResponse was signed by a (valid) IdP
        /// </summary>
        private bool ValidateX509CertificateSignature(XmlDocument SAMLResponse)
        {
            //Retrieve the signature from the SAMLResponse
            XmlNodeList XMLSignatures = SAMLResponse.GetElementsByTagName("Signature",  "http://www.w3.org/2000/09/xmldsig#");

            //If there is no signature in the SAMLResponse, return false
            if (XMLSignatures.Count != 0) {
                return false;
            }

            //Load the X509Certificate into an X509Certificate2 object
            SignedXml SignedSAML = new SignedXml(SAMLResponse);
            SignedSAML.LoadXml( (XmlElement) XMLSignatures[0]);

            //Load the signing certificate into an X509Certificate2 object
            XmlElement x509CertificateElement = (XmlElement) SAMLResponse.GetElementsByTagName("X509Certificate")[0];
            var x509CertificateContent = Convert.FromBase64String(x509CertificateElement.InnerText);
            X509Certificate2 signingCert = new X509Certificate2(x509CertificateContent);
            
            return SignedSAML.CheckSignature(signingCert, true);

            // USING SCHEME INSTEAD BUT NON WORKING
            // Retrieve the signature from the SAMLResponse
            //XmlNodeList XMLSignatures = SAMLResponse.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#");
                       //// If there is no signature in the SAMLResponse, return false
            //if (XMLSignatures.Count == 0)
            //{
            //    return false;
            //}
                       //// Load the signing certificate into an X509Certificate2 object
            //XmlElement x509CertificateElement = (XmlElement)SAMLResponse.GetElementsByTagName("X509Certificate")[0];
            //var x509CertificateContent = Convert.FromBase64String(x509CertificateElement.InnerText);
            //X509Certificate2 signingCert = new X509Certificate2(x509CertificateContent);
                       //// Initialize SAML validation parameters
            //var samlValidationParameters = new Microsoft.IdentityModel.Tokens.Saml2.Saml2ValidationParameters
            //{
            //    AudienceUriMode = Microsoft.IdentityModel.Tokens.AudienceUriMode.Never,
            //    CertificateValidator = System.IdentityModel.Selectors.X509CertificateValidator.None,
            //    RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck,
            //};
                       //// Create a SAML2 Security Token Handler and validate the SAMLResponse signature
            //var samlSecurityTokenHandler = new Microsoft.IdentityModel.Tokens.Saml2.Saml2SecurityTokenHandler();
            //var securityToken = samlSecurityTokenHandler.ReadToken(SAMLResponse.OuterXml);
            //samlSecurityTokenHandler.ValidateToken(securityToken, samlValidationParameters);
                       //return true;
        }
    }
}
