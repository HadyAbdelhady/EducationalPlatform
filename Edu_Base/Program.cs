using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using System.Reflection;

namespace Edu_Base
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Database Configuration
            builder.Services.AddDbContext<EducationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // MediatR Configuration
            builder.Services.AddMediatR(cfg => 
            {
                cfg.RegisterServicesFromAssembly(Assembly.Load("Application"));
            });

            // FluentValidation Configuration
            builder.Services.AddValidatorsFromAssembly(Assembly.Load("Application"));

            // Repository Registration
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // Service Registration
            builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();

            // Google Authentication Configuration
            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? throw new InvalidOperationException("Google ClientId is not configured");
                    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? string.Empty;
                });

            // CORS Configuration (optional - configure as needed)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.Services.AddControllers();
            
            // Swagger/OpenAPI Configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Educational Platform API",
                    Version = "v1",
                    Description = "API for Educational Platform with Google Authentication"
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Educational Platform API v1");
                    c.RoutePrefix = string.Empty; // Swagger UI at root
                });
            }

            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
