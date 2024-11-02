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
                    dataFactory.Seed(context);
                }

                // Test the Retriever class
                Console.WriteLine("\nTesting Retriever Class:");
                Console.WriteLine("------------------------");

                // Create necessary instances
                var indexBuilder = new IndexBuilder(context);
                var retriever = new Retriever(indexBuilder);

                // Test cases matching the provided authorization data
                var testCases = new[]
                {
                    // Client 001 test cases
                    new { ClientId = "001", Method = "GET", Path = "/movie/all", Description = "Get all movies (Admin)" },
                    new { ClientId = "001", Method = "GET", Path = "/movie", Description = "Get single movie (Admin)" },
                    new { ClientId = "001", Method = "POST", Path = "/movie", Description = "Create movie (Admin)" },
                    new { ClientId = "001", Method = "GET", Path = "/person/all", Description = "Get all persons (Admin)" },
                    new { ClientId = "001", Method = "GET", Path = "/person", Description = "Get single person (Admin)" },
                    new { ClientId = "001", Method = "POST", Path = "/person", Description = "Create person (Admin)" },
                    
                    // Client 002 test cases
                    new { ClientId = "002", Method = "GET", Path = "/movie/all", Description = "Get all movies (Limited)" },
                    new { ClientId = "002", Method = "GET", Path = "/movie", Description = "Get single movie (Limited)" },
                    new { ClientId = "002", Method = "GET", Path = "/person/all", Description = "Get all persons (Limited)" },
                    new { ClientId = "002", Method = "GET", Path = "/person", Description = "Get single person (Limited)" }
                };

                foreach (var test in testCases)
                {
                    try
                    {
                        Console.WriteLine($"\nTest Case: {test.Description}");
                        Console.WriteLine($"ClientId: {test.ClientId}");
                        Console.WriteLine($"Method: {test.Method}");
                        Console.WriteLine($"Path: {test.Path}");

                        var authorizedAttributes = retriever.GetAuthorizedAttributes(
                            test.ClientId,
                            test.Method,
                            test.Path
                        );

                        Console.WriteLine("Authorized attributes:");
                        foreach (var attr in authorizedAttributes)
                        {
                            Console.WriteLine($"- {attr}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }

                // Test unauthorized scenarios
                Console.WriteLine("\nTesting Unauthorized Scenarios:");
                Console.WriteLine("------------------------------");

                var unauthorizedTests = new[]
                {
                    new { ClientId = "002", Method = "POST", Path = "/movie", Description = "Attempt to create movie (Unauthorized)" },
                    new { ClientId = "003", Method = "GET", Path = "/movie/all", Description = "Unknown client access" },
                    new { ClientId = "001", Method = "DELETE", Path = "/movie", Description = "Unsupported method" }
                };

                foreach (var test in unauthorizedTests)
                {
                    try
                    {
                        Console.WriteLine($"\nTest Case: {test.Description}");
                        Console.WriteLine($"ClientId: {test.ClientId}");
                        Console.WriteLine($"Method: {test.Method}");
                        Console.WriteLine($"Path: {test.Path}");

                        var authorizedAttributes = retriever.GetAuthorizedAttributes(
                            test.ClientId,
                            test.Method,
                            test.Path
                        );

                        Console.WriteLine("Unexpected success - authorization should have failed");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Expected error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
        // Add this to Program.cs after the Mapper test section
        Console.WriteLine("\nTesting ResponseBuilder Class:");
        Console.WriteLine("--------------------------");

        // Sample movie response with nested data
        string movieCollectionResponse = @"{
            ""movies"": [
                {
                    ""id"": 1,
                    ""title"": ""The Shawshank Redemption"",
                    ""description"": ""Two imprisoned men bond over a number of years..."",
                    ""releaseDate"": ""1994-09-23"",
                    ""totalCost"": 25000000,
                    ""totalBudget"": 20000000,
                    ""crew"": [
                        {
                            ""id"": 101,
                            ""name"": ""Frank Darabont"",
                            ""birthDate"": ""1959-01-28"",
                            ""mostFamousMovieId"": 1
                        },
                        {
                            ""id"": 102,
                            ""name"": ""Tim Robbins"",
                            ""birthDate"": ""1958-10-16"",
                            ""mostFamousMovieId"": 1
                        }
                    ]
                }
            ]
        }";

        string singleMovieResponse = @"{
            ""movie"": {
                ""id"": 1,
                ""title"": ""The Shawshank Redemption"",
                ""description"": ""Two imprisoned men bond over a number of years..."",
                ""releaseDate"": ""1994-09-23"",
                ""totalCost"": 25000000,
                ""totalBudget"": 20000000,
                ""crew"": [
                    {
                        ""id"": 101,
                        ""name"": ""Frank Darabont"",
                        ""birthDate"": ""1959-01-28"",
                        ""mostFamousMovieId"": 1
                    },
                    {
                        ""id"": 102,
                        ""name"": ""Tim Robbins"",
                        ""birthDate"": ""1958-10-16"",
                        ""mostFamousMovieId"": 1
                    }
                ]
            }
        }";

        string personCollectionResponse = @"{
            ""persons"": [
                {
                    ""id"": 101,
                    ""name"": ""Frank Darabont"",
                    ""birthDate"": ""1959-01-28"",
                    ""mostFamousMovieId"": 1,
                    ""mostFamousMovie"": {
                        ""id"": 1,
                        ""title"": ""The Shawshank Redemption"",
                        ""description"": ""Two imprisoned men bond over a number of years..."",
                        ""totalBudget"": 20000000,
                        ""totalCost"": 25000000,
                        ""releaseDate"": ""1994-09-23""
                    },
                    ""workedOn"": [
                        {
                            ""id"": 1,
                            ""title"": ""The Shawshank Redemption"",
                            ""description"": ""Two imprisoned men bond over a number of years..."",
                            ""totalBudget"": 20000000,
                            ""totalCost"": 25000000,
                            ""releaseDate"": ""1994-09-23""
                        }
                    ]
                }
            ]
        }";

        // Create test instances
        var mapper = new Mapper();
        var responseBuilder = new ResponseBuilder(mapper);

        // Test Case 1: Admin (Client 001) - GET /movie/all
        Console.WriteLine("\nTest Case 1: Admin (001) - GET /movie/all");
        var adminMovieAllAttributes = new List<string>
        {
            "$.movies[*].id",
            "$.movies[*].title",
            "$.movies[*].description",
            "$.movies[*].releaseDate",
            "$.movies[*].totalCost",
            "$.movies[*].totalBudget",
            "$.movies[*].crew[*].id",
            "$.movies[*].crew[*].name",
            "$.movies[*].crew[*].birthDate",
            "$.movies[*].crew[*].mostFamousMovieId"
        };

        try
        {
            string adminMovieAllResponse = responseBuilder.BuildResponse(movieCollectionResponse, adminMovieAllAttributes);
            Console.WriteLine("Admin Movie Collection Response:");
            Console.WriteLine(adminMovieAllResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in admin movie collection test: {ex.Message}");
        }

        // Test Case 2: Limited User (Client 002) - GET /movie/all
        Console.WriteLine("\nTest Case 2: Limited User (002) - GET /movie/all");
        var limitedMovieAllAttributes = new List<string>
        {
            "$.movies[*].id",
            "$.movies[*].title",
            "$.movies[*].description",
            "$.movies[*].releaseDate",
            "$.movies[*].crew[*].id",
            "$.movies[*].crew[*].name",
            "$.movies[*].crew[*].mostFamousMovieId"
        };

        try
        {
            string limitedMovieAllResponse = responseBuilder.BuildResponse(movieCollectionResponse, limitedMovieAllAttributes);
            Console.WriteLine("Limited User Movie Collection Response:");
            Console.WriteLine(limitedMovieAllResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in limited user movie collection test: {ex.Message}");
        }

        // Test Case 3: Admin (Client 001) - GET /person/all
        Console.WriteLine("\nTest Case 3: Admin (001) - GET /person/all");
        var adminPersonAllAttributes = new List<string>
        {
            "$.persons[*].id",
            "$.persons[*].name",
            "$.persons[*].birthDate",
            "$.persons[*].mostFamousMovieId",
            "$.persons[*].mostFamousMovie.id",
            "$.persons[*].mostFamousMovie.title",
            "$.persons[*].mostFamousMovie.description",
            "$.persons[*].mostFamousMovie.totalBudget",
            "$.persons[*].mostFamousMovie.totalCost",
            "$.persons[*].mostFamousMovie.releaseDate",
            "$.persons[*].workedOn[*].id",
            "$.persons[*].workedOn[*].title",
            "$.persons[*].workedOn[*].description",
            "$.persons[*].workedOn[*].totalBudget",
            "$.persons[*].workedOn[*].totalCost",
            "$.persons[*].workedOn[*].releaseDate"
        };

        try
        {
            string adminPersonAllResponse = responseBuilder.BuildResponse(personCollectionResponse, adminPersonAllAttributes);
            Console.WriteLine("Admin Person Collection Response:");
            Console.WriteLine(adminPersonAllResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in admin person collection test: {ex.Message}");
        }

        // Test Case 4: Limited User (Client 002) - GET /person/all
        Console.WriteLine("\nTest Case 4: Limited User (002) - GET /person/all");
        var limitedPersonAllAttributes = new List<string>
        {
            "$.persons[*].id",
            "$.persons[*].name",
            "$.persons[*].mostFamousMovieId",
            "$.persons[*].mostFamousMovie.id",
            "$.persons[*].mostFamousMovie.title",
            "$.persons[*].mostFamousMovie.description",
            "$.persons[*].mostFamousMovie.releaseDate",
            "$.persons[*].workedOn[*].id",
            "$.persons[*].workedOn[*].title",
            "$.persons[*].workedOn[*].description",
            "$.persons[*].workedOn[*].releaseDate"
        };

        try
        {
            string limitedPersonAllResponse = responseBuilder.BuildResponse(personCollectionResponse, limitedPersonAllAttributes);
            Console.WriteLine("Limited User Person Collection Response:");
            Console.WriteLine(limitedPersonAllResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in limited user person collection test: {ex.Message}");
        }

        // Test error handling
        Console.WriteLine("\nTest Case 5: Error Handling - Invalid JSON");
        string invalidJson = @"{""movies"": [{""title"": ""Broken Movie"",}}]"; // Note the extra comma

        try
        {
            string errorResponse = responseBuilder.BuildResponse(invalidJson, adminMovieAllAttributes);
            Console.WriteLine("This should not be printed due to error");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Expected error caught: {ex.Message}");
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
