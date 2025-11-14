using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ONO.API.Middleware;
using ONO.Core.Entities;
using ONO.Infrasturcture.DateSeeding;
using ONO.Infrasturcture.Extensions;
using ONO.Infrasturcture.Persistence;
using QuestPDF.Infrastructure;
using Serilog.AspNetCore;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;
using Serilog.Enrichers.WithCaller;
using Serilog.Sinks.SystemConsole.Themes;

namespace ONO.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            #region Log the errors before starting of the app
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.File("logs/app-starting-logs/app-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {SourceContext} {Message}{NewLine}{Exception}")
                .CreateBootstrapLogger();
            #endregion

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpContextAccessor();

            // prevent infinite loop when serializing objects with circular refrences 
            builder.Services.AddControllers().AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            // the license of library that creating the PDF
            QuestPDF.Settings.License = LicenseType.Community;

            #region Hosting Serilog

            builder.Host.UseSerilog((context, services, config) =>
            {
                config
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    // Log everything to the console
                    //.WriteTo.Console(
                    //    theme: AnsiConsoleTheme.Sixteen,
                    //    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}\n\n"
                    //)
                    .WriteTo.Logger(lc => lc
                        .Filter.ByIncludingOnly(e =>
                            e.Properties.ContainsKey("SourceContext") &&
                            !e.Properties["SourceContext"].ToString().Contains("Microsoft.EntityFrameworkCore.Database.Command") &&
                            e.Properties["SourceContext"].ToString().Contains("ONO"))
                        .WriteTo.Console(
                        theme: AnsiConsoleTheme.Sixteen,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}\n")
                    // Log only ONO informations
                    .WriteTo.Logger(lc => lc
                        .Filter.ByIncludingOnly(e =>
                            e.Properties.ContainsKey("SourceContext") &&
                            !e.Properties["SourceContext"].ToString().Contains("Microsoft.EntityFrameworkCore.Database.Command") &&
                            e.Properties["SourceContext"].ToString().Contains("ONO"))
                        .WriteTo.File(
                            path: "logs/application-logs/log-.txt",
                            rollingInterval: RollingInterval.Day,
                            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}\n\n"))
                    // Log only SQL queries to a separate file
                    .WriteTo.Logger(lc => lc
                        .Filter.ByIncludingOnly(e =>
                            e.Properties.ContainsKey("SourceContext") &&
                            e.Properties["SourceContext"].ToString().Contains("Microsoft.EntityFrameworkCore.Database.Command"))
                        .WriteTo.File(
                            path: "logs/sql-logs/sql-.txt",
                            rollingInterval: RollingInterval.Day,
                            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}\n\n")));
            });

            #endregion

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
                        policy.WithOrigins(
                               "http://localhost:5173",
                               "https://ono-lake.vercel.app"
                           )
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

                    //await SeedingRoles.SeedingAsync(userManager, roleManager);
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

            app.UseSerilogRequestLogging();

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
