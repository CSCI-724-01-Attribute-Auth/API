using API;
using API.App_Start;
using API.Data;
using Microsoft.EntityFrameworkCore;


public class Startup 
{
    private readonly string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

    public IConfiguration Configuration { get; set; }

    public Startup(IConfiguration configuration) => this.Configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        var connectionString = "Server=tcp:csci724-project.database.windows.net,1433;Initial Catalog=api-database;Persist Security Info=False;User ID=CSCI724Admin;Password=C$c1Pr0ject4dm!n;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

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

        services.AddDbContext<DBContext>(options => options.UseSqlServer(connectionString));

        services.RegisterDependency();

        services.AddControllersWithViews();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader());

        app.UseStaticFiles();

        app.UseRouting();

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