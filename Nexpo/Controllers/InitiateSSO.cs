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

            // Get the desired identity provider (IdP) endpoint URL
            string idpSSOServiceEndpoint = "https://idpv4.lu.se/idp/profile/Shibboleth/SSO";

            // Create a unique identifier and a timestamp for the SAMLRequest
            string id = "_" + Guid.NewGuid().ToString();
            DateTime now = DateTime.UtcNow;
            string issueInstant = now.ToString("yyyy-MM-ddTHH:mm:ssZ");

            // Create the SAMLRequest XML document
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("samlp", "AuthnRequest", "urn:oasis:names:tc:SAML:2.0:protocol");
            root.SetAttribute("ID", id);
            root.SetAttribute("Version", "2.0");
            root.SetAttribute("IssueInstant", issueInstant);
            root.SetAttribute("ProtocolBinding", "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect");
            root.SetAttribute("AssertionConsumerServiceURL", "https://www.nexpo.arkadtlth.se/api/Saml/ACS");
            root.SetAttribute("Destination", idpSSOServiceEndpoint);
            doc.AppendChild(root);

            XmlElement issuer = doc.CreateElement("saml", "Issuer", "urn:oasis:names:tc:SAML:2.0:assertion");
            issuer.InnerText = "https://www.nexpo.arkadtlth.se/api/saml/SP";
            root.AppendChild(issuer);

            XmlElement nameidPolicy = doc.CreateElement("samlp", "NameIDPolicy", "urn:oasis:names:tc:SAML:2.0:protocol");
            nameidPolicy.SetAttribute("Format", "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified");
            nameidPolicy.SetAttribute("AllowCreate", "true");
            root.AppendChild(nameidPolicy);

            // Sign the SAMLRequest using a private key
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            string privateKeyXml = "<RSAKeyValue><Modulus>...</Modulus><Exponent>...</Exponent><D>...</D><P>...</P><Q>...</Q><DP>...</DP><DQ>...</DQ><InverseQ>...</InverseQ></RSAKeyValue>";
            rsa.FromXmlString(privateKeyXml);
            SignedXml signedXml = new SignedXml(doc);
            signedXml.SigningKey = rsa;
            Reference reference = new Reference("#" + id);
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigExcC14NTransform());
            signedXml.AddReference(reference);
            signedXml.ComputeSignature();
            XmlElement signature = signedXml.GetXml();
            root.AppendChild(signature);

            // Convert the SAMLRequest to a URL-encoded string and redirect the user
            string samlRequest = Convert.ToBase64String(Encoding.UTF8.GetBytes(doc.OuterXml));
            string redirectUrl = idpSSOServiceEndpoint + "?SAMLRequest=" + HttpUtility.UrlEncode(samlRequest);
            return Redirect(redirectUrl);
            
            
                
        }
    }
}

