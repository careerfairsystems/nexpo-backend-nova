using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Nexpo.Models;
using Microsoft.EntityFrameworkCore;
using Nexpo.Repositories;
using Nexpo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using ITfoxtec.Identity.Saml2.MvcCore.Configuration;
using System.Linq;

// maybe dublicated imports
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using ITfoxtec.Identity.Saml2.MvcCore.Configuration;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using Microsoft.AspNetCore.Authentication;
using ITfoxtec.Identity.Saml2.Schemas;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace Nexpo
{
    public class Startup
    {
        public IConfig Config { get; }
        public IWebHostEnvironment Environment { get; }

        public readonly string CorsPolicy = nameof(CorsPolicy);

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Config = new Config(configuration);
            Environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Config.JWTIssuer,
                    ValidAudience = Config.JWTAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Config.SecretKey))
                };
            });

            services.AddScoped<IConfig>(_ => Config);
            services.AddDbContext<ApplicationDbContext>(opt => opt.UseNpgsql(Config.ConnectionString));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<ICompanyConnectionRepository, CompanyConnectionRepository>();
            services.AddScoped<IStudentSessionTimeslotRepository, StudentSessionTimeslotRepository>();
            services.AddScoped<IStudentSessionApplicationRepository, StudentSessionApplicationRepository>();
            services.AddScoped<IStudentSessionRepository, StudentSessionRepository>();

            services.AddScoped<PasswordService, PasswordService>();
            services.AddScoped<TokenService, TokenService>();
            services.AddScoped<FileService, FileService>();
            services.AddHttpContextAccessor();
            if (Environment.IsDevelopment())
            {
                services.AddScoped<IEmailService, DevEmailService>();

            }
            else
            {
                services.AddScoped<IEmailService, EmailService>();
            }

            services.AddCors(options =>
            {
                options.AddPolicy(name: CorsPolicy, builder =>
                {
                    builder.AllowAnyMethod();
                    builder.AllowAnyOrigin();
                    builder.AllowAnyHeader();
                });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Nexpo", Version = "v1" });
            });

            services.Configure<Saml2Configuration>(saml2Configuration =>
            {
                //saml2Configuration.SignAuthnRequest = true;
                //saml2Configuration.SigningCertificate = CertificateUtil.Load(AppEnvironment.MapToPhysicalFilePath(Config["Saml2:SigningCertificateFile"]), Configuration["Saml2:SigningCertificatePassword"], X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

                //saml2Configuration.SignatureValidationCertificates.Add(CertificateUtil.Load(AppEnvironment.MapToPhysicalFilePath(Configuration["Saml2:SignatureValidationCertificateFile"])));
                saml2Configuration.AllowedAudienceUris.Add(saml2Configuration.Issuer);

                var entityDescriptor = new EntityDescriptor();
                entityDescriptor.ReadIdPSsoDescriptorFromUrl(new Uri(Config.IdPMetadata));
                if (entityDescriptor.IdPSsoDescriptor != null)
                {
                    saml2Configuration.AllowedIssuer = entityDescriptor.EntityId;
                    saml2Configuration.SingleSignOnDestination = entityDescriptor.IdPSsoDescriptor.SingleSignOnServices.First().Location;
                    saml2Configuration.SingleLogoutDestination = entityDescriptor.IdPSsoDescriptor.SingleLogoutServices.First().Location;
                    saml2Configuration.SignatureValidationCertificates.AddRange(entityDescriptor.IdPSsoDescriptor.SigningCertificates);
                }
                else
                {
                    throw new Exception("IdPSsoDescriptor not loaded from metadata.");
                }

            });

            services.AddSaml2(slidingExpiration: true);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nexpo v1"));

                dbContext.Database.Migrate();
                dbContext.Seed();
            }

            app.UseRouting();
            app.UseSaml2();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(CorsPolicy);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

            });

            //Require SAML 2.0 authorization before SPA load.
            app.Use(async (context, next) =>
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    await context.ChallengeAsync(Saml2Constants.AuthenticationScheme);
                }
                else
                {
                    await next();
                }
            });


        }
    }
}

