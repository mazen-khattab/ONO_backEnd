using CurrencyExchange_Practice.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using ONO.Infrasturcture.Persistence;
using ONO.Infrasturcture.Extensions;
using ONO.Core.Entities;

namespace ONO.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            #region Registration
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

            builder.Services.AddInfrastructureServices(connectionString);
            #endregion

            #region Authentication
            builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedEmail = false;
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = false;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                    };
                });
            #endregion

            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                options.RoutePrefix = ""; // Serve Swagger UI at the app's root
            });

            app.MapControllers();

            app.Run();
        }
    }
}
