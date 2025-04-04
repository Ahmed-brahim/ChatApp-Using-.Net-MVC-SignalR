using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SignalR_Project.Hubs;
using SignalR_Project.Models;
using SignalR_Project.Models.Data;

namespace SignalR_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            { 
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true; })
                .AddEntityFrameworkStores<AppDbContext>();

            builder.Services.AddDbContext<AppDbContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("Cs"))
                .AddInterceptors(new SoftDeleteInterceptor()) // Add the interceptor to the DbContext
                );
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<ChatHub>("/chatHub"); //map the ChatHub to the /chatHub endpoint

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
