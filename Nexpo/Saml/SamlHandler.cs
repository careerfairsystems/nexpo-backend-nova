using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sustainsys.Saml2.AspNetCore2;

namespace Nexpo.Saml
{
    public class SamlHandler : Saml2Handler
    {
        private readonly IOptions<Saml2Options> _options;
        private readonly ILogger _logger;

        public SamlHandler(IOptionsMonitorCache<Saml2Options> optionsCache, IDataProtectionProvider dataProtectorProvider, IOptionsFactory<Saml2Options> optionsFactory) 
        : base(optionsCache, dataProtectorProvider, optionsFactory)
        {
            
        }

        public AuthenticateResult HandleAuthenticateAsync(AuthenticateContext context){
            throw new NotImplementedException();
        }

        public Task HandleSignOutAsync(SignOutContext context){
            throw new NotImplementedException();
        }



    }

}
