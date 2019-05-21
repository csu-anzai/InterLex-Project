using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Interlex
{
    using System.Text;
    using AutoMapper;
    using Data;
    using Interlex.Common;
    using Interlex.Middleware;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.IdentityModel.Tokens;
    using Models.ResponseModels;
    using MvcConfiguration;
    using Repo;
    using Serilog;
    using Services;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureLoggerService(this.Configuration);
            //services.AddResponseCaching();
            services.AddMvc(options =>
                {
                    options.ModelMetadataDetailsProviders.Add(new RequiredBindingMetadataProvider()); // required attribute will work as BindingRequired for forms, query, not json!
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddHttpContextAccessor(); // not needed ? 
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
                    .ConfigureWarnings(warn =>
                    {
                        warn.Throw(RelationalEventId.QueryClientEvaluationWarning);
                        warn.Throw(CoreEventId.NavigationLazyLoading);
                    });
            });
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    //options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            services.AddCors();
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = "https://www.apis.bg",
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration.GetValue<string>("SigningKey")))
                    };
                });
            services.AddAuthorization(config =>
            {
                config.AddPolicy("Admin", builder => builder.RequireClaim(Constants.Privileges, Constants.Admin, Constants.SuperAdmin));
                config.AddPolicy("SuperAdmin", builder => builder.RequireClaim(Constants.Privileges, Constants.SuperAdmin));
            });
            services.AddAutoMapper();
            this.AddCustomServices(services);

        }

        private void AddCustomServices(IServiceCollection services)
        {
            services.AddSingleton<CurrentUserService>();
            services.AddScoped<UsersService>();
            services.AddScoped<CaseService>();
            services.AddScoped<OrganizationService>();
            services.AddScoped<Repository>();
            services.AddSingleton<AknConvertService>();
            services.AddSingleton(implementationFactory: _ => new ReferenceEditorRepository(this.Configuration.GetCrawlerStorageConnection()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseExceptionMiddleware();

            if (env.IsDevelopment())
            {
                // app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //SeedDatabase.Initialize(app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider).Wait();
            app.UseHttpsRedirection();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseAuthentication();
            //app.UseNoCacheMiddleware();
            app.UseMvc();
        }


        private static void ConfigureLoggerService(IConfiguration configuration)
        {
            // the logger usage is registered in Program.cs -> BuildWebHost -> UseSerilog()

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            Log.Logger = logger;
        }
    }
}