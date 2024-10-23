using API;
using API.Data;

public class Program
{
    public static void Main(string[] args)
    {
        var app = CreateHostBuilder(args).Build();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<DBContext>();
                var dataFactory = services.GetRequiredService<DataFactory>();

                // Check if the database is empty and seed it
                if (!context.Movies.Any())
                {
                    dataFactory.Seed(context); // Call your seeding logic here
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        app.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) => 
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
