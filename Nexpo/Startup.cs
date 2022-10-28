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
using Nexpo.AWS;
// adding for SAML feature
using Nexpo.Constants;
using Nexpo.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Sustainsys.Saml2;
using Sustainsys.Saml2.Metadata;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Nexpo
{
    public class Startup
    {
        public IConfig Config { get; }
        public IWebHostEnvironment Environment { get; }

        public readonly string CorsPolicy = nameof(CorsPolicy);

        public static class AuthenticationHelpers
        {
            public static void CheckSameSite(HttpContext httpContext, CookieOptions options)
            {
                if (options.SameSite != SameSiteMode.None)
                    return;
                string userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                if (httpContext.Request.IsHttps && !AuthenticationHelpers.DisallowsSameSiteNone(userAgent))
                    return;
                options.SameSite = SameSiteMode.Unspecified;
            }

            public static bool DisallowsSameSiteNone(string userAgent) => userAgent.Contains("CPU iPhone OS 12") || userAgent.Contains("iPad; CPU OS 12") || userAgent.Contains("Macintosh; Intel Mac OS X 10_14") && userAgent.Contains("Version/") && userAgent.Contains("Safari") || userAgent.Contains("Chrome/5") || userAgent.Contains("Chrome/6");
        }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Config = new Config(configuration);
            Environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // ** ADDED for SSO feature **
            services.AddControllers();
            //services.ConfigureNonBreakingSameSiteCookies();
            services.Configure<CookiePolicyOptions>(options =>
            {
                //SameSiteMode.None is required to support SA
                //options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
	    	options.MinimumSameSitePolicy = SameSiteMode.None;
                //options.Secure = CookieSecurePolicy.Always;
                options.CheckConsentNeeded = context => false;
                // Some older browsers don't support SameSiteMode.None
                options.OnAppendCookie = cookieContext =>
                    AuthenticationHelpers.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                    AuthenticationHelpers.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });


            services.AddSession(options =>
            {
                //options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.None;
                //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

            });
            /*services.AddMvc((options) =>
            {
                options.RespectBrowserAcceptHeader = true;
                options.ReturnHttpNotAcceptable = true;
                options.InputFormatters.Add(new XmlSerializerInputFormatter(
                    options));
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                options.FormatterMappings.SetMediaTypeMappingForFormat("json", "application/json");
                options.FormatterMappings.SetMediaTypeMappingForFormat("xml", "application/xml");
            })
            .AddDataAnnotationsLocalization();*/
            // ** **
            
            services.AddControllers();
            services.AddSingleton<IS3Configuration, S3Config>();
            services.AddScoped  <IAws3Services> (_ => new Aws3Services("AKIAX3BYI22ZD733TJZ3","Zz6i8UUK3FH003JjnvzqtQTjb7SMg9qxV2CSCfBK","eu-north-1","cvfiler")) ;
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                // add for SSO
                o.DefaultScheme = ApplicationSamlConstants.Application;
                o.DefaultSignInScheme = ApplicationSamlConstants.External;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // maybe false
                    ValidateAudience = false, // maybe false
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Config.JWTIssuer,
                    ValidAudience = Config.JWTAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Config.SecretKey))
                };
            })
            .AddCookie(ApplicationSamlConstants.Application)
            .AddCookie(ApplicationSamlConstants.External)
            .AddSaml2(options =>
            {
                options.SPOptions.EntityId = new EntityId(this.Config.SPEntityId);
		        options.SPOptions.PublicOrigin = new Uri("https://www.nexpo.arkadtlth.se");
                        //options.SPOptions.ReturnUrl = new Uri("https://www.nexpo.arkadtlth.se/");
		        options.IdentityProviders.Add(
                            new IdentityProvider(
                                new EntityId(this.Config.IDPEntityId), options.SPOptions)
                            {
                                LoadMetadata = true,
			        AllowUnsolicitedAuthnResponse = true,
			        //Binding = Sustainsys.Saml2.WebSso.Saml2BindingType.HttpRedirect
			
			        //RelayStateUsedAsReturnUrl = true
                            });
		        options.SPOptions.WantAssertionsSigned = false;

                        //options.SPOptions.ServiceCertificates.Add(new X509Certificate2(this.Config.CertificatePath, this.Config.CertificatePassword));

                options.SPOptions.ServiceCertificates.Add(
                            new ServiceCertificate
                            {
                                Certificate = new X509Certificate2(
                                    this.Config.CertificatePath,
                                    this.Config.CertificatePassword),
                                Use = CertificateUse.Both
                            });
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


            services.AddScoped<PasswordService, PasswordService>();
            services.AddScoped<TokenService, TokenService>();
            services.AddScoped<FileService, FileService>();
            if (Environment.IsDevelopment())
            {
                services.AddScoped<IEmailService, DevEmailService>();
            }
            else
            {
                services.AddScoped<IEmailService, EmailService>();
            }

            services.AddCors();
            /*            services.AddCors(options =>
                        {
                            options.AddPolicy(name: CorsPolicy, builder =>
                            {
                                builder.AllowAnyMethod();
                                builder.AllowAnyOrigin();
                                builder.AllowAnyHeader();
                            });
                        });*/

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Nexpo", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext dbContext)
        {
            app.UseRouting();
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nexpo v1"));

                dbContext.Database.Migrate();
                dbContext.Seed();
            }

	    //dbContext.Database.Migrate(); //ADD THIS IF DATABASoE IS CLOSED        
            //app.UseCookiePolicy();
	    app.UseAuthorization();
	   
	    app.UseSession();
            app.UseAuthentication();
	    
            //app.UseHttpsRedirection();
            

            // MAYBE NEED TO USEMVC AND CHANGE ACCORDING TO GIT
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }

    }
}

