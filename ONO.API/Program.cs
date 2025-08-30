using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using ONO.Infrasturcture.Persistence;
using ONO.Infrasturcture.Extensions;
using ONO.Core.Entities;
using System.Threading.Tasks;
using ONO.Infrasturcture.DateSeeding;
using System.Text.Json.Serialization;
using QuestPDF.Infrastructure;
using ONO.API.Middleware;

namespace ONO.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddControllers().AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            builder.Services.AddSwaggerGen();

            QuestPDF.Settings.License = LicenseType.Community;

            builder.Services.AddHttpContextAccessor();

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
                options.Password.RequireNonAlphanumeric = true; //@, #, $, %, !, ?, and other special characters
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                 .AddJwtBearer(options =>
                 {
                     options.Events = new JwtBearerEvents
                     {
                         OnMessageReceived = context =>
                         {
                             context.Token = context.Request.Cookies["accessToken"];
                             return Task.CompletedTask;
                         }
                     };

                     options.SaveToken = false;
                     options.RequireHttpsMetadata = false;
                     options.TokenValidationParameters = new()
                     {
                         ValidateIssuer = true,
                         ValidIssuer = builder.Configuration["Jwt:Issuer"],
                         ValidateAudience = true,
                         ValidAudience = builder.Configuration["Jwt:Audience"],
                         ValidateIssuerSigningKey = true,
                         RequireExpirationTime = true,
                         ValidateLifetime = true,
                         ClockSkew = TimeSpan.Zero,
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                     };
                 });
            #endregion

            #region CORS

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "FrontendPolicy",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:5173")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });

            #endregion

            var app = builder.Build();

            #region DataSeeding
            try
            {
                using (var scope = app.Services.CreateScope())
                {
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

                    await SeedingRoles.SeedingAsync(userManager, roleManager);
                }
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An Error Occurs When Applying The Migrations");
            }

            #endregion

            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors("FrontendPolicy");
            app.UseAuthentication();
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
