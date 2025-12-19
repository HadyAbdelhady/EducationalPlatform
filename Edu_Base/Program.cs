using Application.DTOs.Media;
using Application.Interfaces;
using CloudinaryDotNet;
using FluentValidation;
using Infrastructure.Data;
using Infrastructure.Middleware;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.ReviewService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace Edu_Base
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Register EF Core interceptor used by the DbContext
            builder.Services.AddSingleton<SoftDeleteInterceptor>();


            // Database Configuration (register DbContext with interceptor from DI)
            builder.Services.AddDbContext<EducationDbContext>((provider, options) =>
            {
                // Enable a retry-on-failure execution strategy and set a command timeout
                options.UseNpgsql(
                        builder.Configuration.GetConnectionString("DefaultConnection"),
                        npgsqlOptions => npgsqlOptions
                            .EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(2), errorCodesToAdd: null)
                            .CommandTimeout(60)
                    )
                       .AddInterceptors(provider.GetRequiredService<SoftDeleteInterceptor>());
            });

            // MediatR Configuration
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.Load("Application"));
            });

            // Configure Cloudinary
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

            builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("Authentication:Google")
                            );

            // Register Cloudinary instance
            builder.Services.AddSingleton(provider =>
            {
                var config = provider.GetRequiredService<IOptions<CloudinarySettings>>();
                var account = new Account(
                    config.Value.CloudName,
                    config.Value.ApiKey,
                    config.Value.ApiSecret
                );
                return new Cloudinary(account);
            });

            // FluentValidation Configuration
            builder.Services.AddValidatorsFromAssembly(Assembly.Load("Application"));

            // Unit of Work & Repository Registration
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Open generic base repository
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Auto-register concrete repository implementations (Scrutor)
            builder.Services.Scan(scan => scan
                .FromAssembliesOf(typeof(UnitOfWork))
                .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Repository")))
                .AsImplementedInterfaces()
                .WithScopedLifetime());


            // Service Registration
            builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            builder.Services.AddScoped<ICloudinaryCore, CloudinaryService>();
            builder.Services.AddScoped<IReviewServiceFactory, ReviewServiceFactory>();
            builder.Services.AddScoped<IReviewService, CourseReviewService>();
            builder.Services.AddScoped<IReviewService, SectionReviewService>();
            builder.Services.AddScoped<IReviewService, InstructorReviewService>();
            builder.Services.AddScoped<IReviewService, VideoReviewService>();
            builder.Services.AddScoped<IQuestionUpdateService, QuestionUpdateService>();

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

            // JWT Authentication Configuration
            var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "EducationalPlatform";
            var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "EducationalPlatformUsers";

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAuthorization();

            builder.Services.AddControllers();

            // Swagger/OpenAPI Configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Educational Platform API",
                    Version = "v1",
                    Description = "API for Educational Platform with Google Authentication and JWT"
                });

                // Add JWT Authentication to Swagger
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Educational Platform API v1");
                c.RoutePrefix = string.Empty; // Swagger UI at root
            });


            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseScreenshotCheck();

            app.MapControllers();

            app.Run();
        }
    }
}
