using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexpo.Tests
{
    class MockConfig : IConfig
    {
        public string BaseUrl { get; set; }
        public string SendGridApiKey { get; set; }
        public string SecretKey { get; set; }
        public string JWTIssuer { get; set; }
        public string JWTAudience { get; set; }
        public string JWTExpires { get; set; }
        public string ConnectionString { get; set; }
    }
}
