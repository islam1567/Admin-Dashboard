
using Admin_Dashboard.Core.DbContext;
using Admin_Dashboard.Core.Entities;
using Admin_Dashboard.Core.Interfaces;
using Admin_Dashboard.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Admin_Dashboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddControllers().
                AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });


            var connection = builder.Configuration.GetConnectionString("CS");
            builder.Services.AddDbContext<ApplecationDbContext>(e =>
            e.UseSqlServer(connection)
            );

            builder.Services.AddIdentity<ApplecationUser, IdentityRole>().
                AddEntityFrameworkStores<ApplecationDbContext>().
                AddDefaultTokenProviders();

            builder.Services.AddScoped<ILogService, LogService>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<IAuthService, AuthService>();


            var app = builder.Build();

            app.UseCors(e =>
            {
                e.AllowAnyHeader().
                AllowAnyMethod().
                AllowAnyOrigin();
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
