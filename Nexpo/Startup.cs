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
using Microsoft.AspNetCore.Http;
using Sustainsys.Saml2;
using Sustainsys.Saml2.Metadata;
using System.Security.Cryptography.X509Certificates;
using Sustainsys.Saml2.AspNetCore2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Formatters;
using Nexpo.Saml;
using Microsoft.AspNetCore.Authentication;


namespace Nexpo
{

    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Config = new Config(configuration);
            Environment = environment;
            Configuration = configuration;
        }

        public static IConfig Config { get; set; }

        public static IConfiguration Configuration { get; set; }
        
        public static IWebHostEnvironment Environment { get; set; }

        public static IServiceProvider ServiceProvider { get; }

        public static readonly string CorsPolicy = nameof(CorsPolicy);

        
        
        //Flytta till folder
        
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public static void ConfigureServices(IServiceCollection services)
        {
            // ** ADDED for SSO feature **
            services.AddDistributedMemoryCache();
            
            services.AddSession(options =>
            {
                //options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.None;
                //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

            });

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
            
            
            
            
            services.AddControllers();
            services.AddSingleton<IS3Configuration, S3Config>();
            services.AddScoped  <IAws3Services> (_ => new Aws3Services("AKIAX3BYI22ZD733TJZ3","Zz6i8UUK3FH003JjnvzqtQTjb7SMg9qxV2CSCfBK","eu-north-1","cvfiler")) ;

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true; //make all urls lowercase
            });

            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //    
            //    options.DefaultScheme = ApplicationSamlConstants.Application;
            //    options.DefaultSignInScheme = ApplicationSamlConstants.External;

            //}).AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true, 
            //        ValidateAudience = true, 
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = Config.JWTIssuer,
            //        ValidAudience = Config.JWTAudience,
            //        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Config.SecretKey))
            //    };
            //})
            //.AddCookie(ApplicationSamlConstants.Application)
            //.AddCookie(ApplicationSamlConstants.External)
            
            services
            .AddAuthentication(options =>
            {
                options.DefaultScheme             = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme       = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme      = CookieAuthenticationDefaults.AuthenticationScheme;
                
                options.DefaultChallengeScheme    = Saml2Defaults.Scheme;
                options.DefaultAuthenticateScheme = Saml2Defaults.Scheme;

            })
            .AddCookie() //this is required for the redirect to work
            //with .addSaml2, we create an saml2 authentication scheme, which is used to authenticate the user
            .AddSaml2(options =>
            {
                options.SPOptions.EntityId = new EntityId(Configuration["SAML:SP:SPEntityId"]);
                options.SPOptions.ReturnUrl = new Uri(Configuration["SAML:SP:SPACSUrl"]);
                //options.SPOptions.SingleLogoutDestination = new Uri(Configuration["SAML:SP:SPLogoutUrl"]);
                //add later

                options.SPOptions.WantAssertionsSigned = false;
                
                options.SPOptions.Saml2PSecurityTokenHandler = new CustomSecurityTokenHandler();

                //var certificate = new X509Certificate2(
                //    Configuration["SAML:SP:SPCertificatePath"],
                //    Configuration["SAML:SP:SPCertificatePassword"]
                //    );

                var certificate = X509Certificate2.CreateFromPemFile(Config.SPCertificatePath, Config.SPPrivateKeyPath);         

                options.SPOptions.ServiceCertificates.Add(new ServiceCertificate
                {
                    Certificate = certificate,
                    Use = CertificateUse.Signing
                });

                options.IdentityProviders.Add(
                    new IdentityProvider(
                        new EntityId(Configuration["SAML:IDP:IDPEntityId"]), options.SPOptions)
                    {
                        LoadMetadata = true,
                        MetadataLocation = Configuration["SAML:IDP:MetadataLocation"],
                        //AllowUnsolicitedAuthnResponse = true,
                        //Binding = Sustainsys.Saml2.WebSso.Saml2BindingType.HttpRedirect,
                        //SingleSignOnServiceUrl = new Uri(Configuration["SAML:IDP:IDPSSOUrl"])
                        //last 3 unnecesary due to loadMetadata?
                    }
                );

                

                
            });

            services.AddMvc((options) =>
            {
                options.RespectBrowserAcceptHeader = true;
                options.ReturnHttpNotAcceptable = true;
                options.InputFormatters.Add(new XmlSerializerInputFormatter(options));
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                options.FormatterMappings.SetMediaTypeMappingForFormat("json", "application/json");
                options.FormatterMappings.SetMediaTypeMappingForFormat("xml", "application/xml");
            })
            .AddDataAnnotationsLocalization();

            services.AddHttpContextAccessor();

            //var serviceProviderOptions = new ServiceProviderOptions();

            //var serviceProvider = services.BuildServiceProvider();
            
            //services.AddScoped<IServiceProvider, SamlController>();
            services.AddScoped<IConfig>(_ => Config);
            services.AddDbContext<ApplicationDbContext>(opt => opt.UseNpgsql(Config.ConnectionString));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IStudentSessionTimeslotRepository, StudentSessionTimeslotRepository>();
            services.AddScoped<IStudentSessionApplicationRepository, StudentSessionApplicationRepository>();
            
            services.AddScoped<PasswordService, PasswordService>();
            services.AddScoped<TokenService, TokenService>();
            services.AddScoped<FileService, FileService>();
            services.AddScoped<IAuthenticationService, SamlAuthService>();

            
            if (Environment.IsDevelopment())
            {
                services.AddScoped<IEmailService, DevEmailService>();
            }
            else
            {
                services.AddScoped<IEmailService, EmailService>();
            }

            services.AddCors();
            //services.AddCors(options =>
            //{
            //    options.AddPolicy(name: CorsPolicy, builder =>
            //    {
            //        builder.AllowAnyMethod();
            //        builder.AllowAnyOrigin();
            //        builder.AllowAnyHeader();
            //    });
            //});

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Nexpo", Version = "v1" });
                
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext dbContext)
        {
            app.UseRouting();

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
            );
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options => 
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Nexpo v1");
                });

                dbContext.Database.Migrate();
                dbContext.Seed();
            }
    
            app.UseCookiePolicy();
            
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


