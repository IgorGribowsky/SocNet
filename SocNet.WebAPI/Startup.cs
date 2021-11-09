using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            //services.AddSingleton<IPostsManagingService, FakePostsManagingService>();
            //services.AddSingleton<IUsersManagingService, FakeUsersManagingService>();

            services.AddTransient<IPostsManagingService, PostsManagingService>();
            services.AddTransient<IUsersManagingService, UsersManagingService>();

            services.AddTransient<IRepository, Repository>();
            services.AddControllers();

            var dbType = Environment.GetEnvironmentVariable("DB_TYPE");
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

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

            if (dbType == "POSTGRE")
            {
                services.AddEntityFrameworkNpgsql()
                    .AddDbContext<ApplicationContext>
                    (options => 
                    options.UseNpgsql(connectionString));
            }
            else
            {

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
