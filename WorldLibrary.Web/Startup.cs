using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vereyon.Web;
using WorldLibrary.Web.Data;
using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Helper;
using WorldLibrary.Web.Repositories;
using Microsoft.Extensions.Azure;
using Azure.Storage.Queues;
using Azure.Storage.Blobs;
using Azure.Core.Extensions;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;

namespace WorldLibrary.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(cfg =>
            {

                cfg.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
                cfg.SignIn.RequireConfirmedEmail = true;
                cfg.User.RequireUniqueEmail = true;
                cfg.Password.RequireDigit = false;
                cfg.Password.RequiredUniqueChars = 0;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequiredLength = 6;

            })
               .AddDefaultTokenProviders()
               .AddEntityFrameworkStores<DataContext>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme=CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme=GoogleDefaults.AuthenticationScheme;
            })
                .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
                {
                    options.ClientId=Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret=Configuration["Authentication:Google:ClientSecret"];
                    options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
                })
            .AddFacebook(options=>
            {
                options.AppId="616234630717235";
                options.AppSecret="a6670e162c79744dad1c754fa21e7356";
            } )

            .AddCookie()
            .AddJwtBearer(cfg =>
            {
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = this.Configuration["Token:Issuer"],
                    ValidAudience = this.Configuration["Token:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(this.Configuration["Tokens:Key"]))
                };

            });
            

            services.AddSession();

            //services.Configure<CookiePolicyOptions>(options =>
            //{                
            //    options.CheckConsentNeeded = context => true;
                
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});

            services.AddDbContext<DataContext>(cfg =>
            {
                cfg.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection2"));
            });

            services.AddFlashMessage();

            services.AddTransient<SeedDb>();

            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IPhysicalLibraryRepository, PhysicalLibraryRepository>();
            services.AddScoped<IReserveRepository, ReserveRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IAssessmentRepository, AssessmentRepository>();
           

            services.AddScoped<IImageHelper, ImageHelper>();
            services.AddScoped<IConverterHelper, ConverterHelper>();
            services.AddScoped<IUserHelper, UserHelper>();
            services.AddScoped<IMailHelper, MailHelper>();
            services.AddScoped<IBlobHelper, BlobHelper>();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/NotAuthorized";
                options.AccessDeniedPath = "/Account/NotAuthorized";

            });

            services.AddControllersWithViews();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Errors/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {             

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                

            });
        }
    }
    
}
