using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using SocNet.Services.PostsManaging;
using SocNet.Services.UsersManaging;
using SocNet.Infrastructure.EFRepository;
using SocNet.Infrastructure.Interfaces;
using SocNet.Services.AuthenticationManaging;
using SocNet.Core.Entities;

namespace SocNet.WebAPI
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
            services.AddTransient<IPostsManagingService, PostsManagingService>();
            services.AddTransient<IUsersManagingService, UsersManagingService>();

            services.AddTransient<IAuthenticationService, AuthenticationMonoliticService>();
            services.AddTransient<IJwtManagingService, JwtManagingService>();

            services.AddTransient<IPasswordHasher<UserIdentity>, PasswordHasher<UserIdentity>>();

            services.AddTransient<IRepository, Repository>();
            services.AddControllers();

            var dbType = Environment.GetEnvironmentVariable("DB_TYPE");
            if (string.IsNullOrEmpty(dbType))
                throw new Exception("Can't find DB_TYPE env var");

            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("Can't find CONNECTION_STRING env var");

            switch (dbType)
            {
                case "MSSQL":
                    services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectionString));
                    break;

                case "POSTGRE":
                    services.AddEntityFrameworkNpgsql()
                    .AddDbContext<ApplicationContext>
                    (options =>
                    options.UseNpgsql(connectionString));
                    break;

                default:
                    throw new Exception("DB_TYPE env var not recognized");
            }

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SocNet.WebAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SocNet.WebAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
