using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

namespace Restaurant_page
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("AppDbContext")));

            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/Account/Login");
                options.AccessDeniedPath = new PathString("/Account/NoAccess");
                options.LogoutPath = new PathString("/index");
            });

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            CreateRoles(serviceProvider).Wait();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc();
            

            app.Run((context) =>
            {
                throw new Exception("Error 404: file not found");
            });
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.
                GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.
                GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Admin", "Member" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var roleResult = await RoleManager.
                        CreateAsync(new IdentityRole(roleName));
                }
            }
            var _user = await UserManager.FindByEmailAsync("admin@chester.ac.uk");
            if (_user != null)
            {
                await UserManager.AddToRoleAsync(_user, "Admin");
            }
        }

    }
}
