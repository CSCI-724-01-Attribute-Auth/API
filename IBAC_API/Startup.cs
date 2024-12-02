using API;
using API.App_Start;
using API.Authorization;
using API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;


public class Startup
{
    private readonly string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

    public IConfiguration Configuration { get; set; }

    public Startup(IConfiguration configuration) => this.Configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        var connectionString = "Server=tcp:csci724-project.database.windows.net,1433;Initial Catalog=api-database;Persist Security Info=False;User ID=CSCI724Admin;Password=C$c1Pr0ject4dm!n;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=120;";

        services.AddCors(options =>
        {
            options.AddPolicy(
                this.myAllowSpecificOrigins,
                builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod();
                });
        });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var key = Encoding.ASCII.GetBytes("your_very_secret_key_here");
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false, // Adjust according to your needs
                ValidateAudience = false, // Adjust according to your needs
                ValidateLifetime = true
            };
        });

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

        services.AddDbContext<DBContext>(options => options.UseSqlServer(connectionString));

        services.RegisterDependency();

        services.AddControllersWithViews();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader());

        app.UseStaticFiles();

        app.UseRouting();

        app.UseMiddleware<AttributeAuthorizer>();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}"
            );
        });
    }
}