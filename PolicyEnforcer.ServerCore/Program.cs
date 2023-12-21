using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using PolicyEnforcer.ServerCore;
using PolicyEnforcer.ServerCore.Database.Context;
using System.Reflection;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSignalR(o => {
            o.EnableDetailedErrors = true;
            o.MaximumReceiveMessageSize = 100240;
        }).AddNewtonsoftJsonProtocol(opts =>
                opts.PayloadSerializerSettings.TypeNameHandling = TypeNameHandling.Auto);

        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = AuthOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = AuthOptions.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                    ValidateIssuerSigningKey = true
                });

        builder.Services.AddDbContext<PolicyEnforcerContext>();
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "PolicyEnforcerAPI", Version = "v1" });

            //// Set the comments path for the Swagger JSON and UI.
            //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //c.IncludeXmlComments(xmlPath);
        });

        var configBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json");

        var config = configBuilder.Build(); 
        var url = config.GetValue<string>("WorkingURL");

        builder.WebHost.UseUrls(url);
        var app = builder.Build();

        //if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                //options.RoutePrefix = string.Empty;
            });
        }
        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.MapControllers();
        app.MapHub<DataCollectionHub>("/data");  

        app.Run();
    }
}