
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using VillaApi.Data;
using VillaApi.Logging;
using VillaApi.Repository;
using VillaApi.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.ApiExplorer;


namespace VillaApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var key=builder.Configuration.GetValue<string>("ApiSettings:Secret");
            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>(option => {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConn"));
            });
            builder.Services.AddScoped<IVillaRepo, VillaRepo>();
            builder.Services.AddScoped<IVillaNumberRepo, VillaNumberRepo>();
            builder.Services.AddScoped<IUserRepo, UserRepo>();
            builder.Services.AddAutoMapper(typeof(MappingConfig));
            /*builder.Services.AddApiVersioning(
                options =>
                {
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                });*/
         /*   builder.Services.AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });*/
            builder.Services.AddAuthentication(
                options=>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(
                o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = true;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            /* Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
                 .WriteTo.File("log/villa.txt",rollingInterval:RollingInterval.Day).CreateLogger();
             builder.Host.UseSerilog();*/
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
              
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                    Description =  "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
                       "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                        "Example: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "Bearer"
                    });
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header
                            },
                            new List<string> ()
                        }
                    });
                }
                );
           // builder.Services.AddSingleton<ILogging,Logging.Logging>();
            var app = builder.Build();

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
