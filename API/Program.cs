using API; 
internal class Program
{
    private static void Main(string[] args)
    {

        // WebApplicationBuilder is used to configure the services and the HTTP request pipeline for the application
        var builder = WebApplication.CreateBuilder(args);

        // Build the application, configuring all middleware, services, and routes
        var app = builder.Build();

        // Load authorization records
        var loader = new Loader("./authorizationRecords.json");
        var authorizationRecords = loader.LoadAuthorizationRecords();

        // Print out the loaded records for verification
        foreach (var record in authorizationRecords)
        {
            Console.WriteLine($"Client: {record.ClientId}, Path: {record.Path}, Attributes: {string.Join(", ", record.AuthorizedAttributes)}");
        }

        // Grouping API routes under the common path "/api" to organize related endpoints
        // This line is used in ASP.NET Core applications to create a new group of API endpoints that share a common prefix in their routing
        // the variable apiGroup holds the result of the MapGroup method
        // The MapGroup method call creates a new route group that prefixes all endpoints defined under this group with /api
        var apiGroup = app.MapGroup("/api");

        // Instantiating a DataFactory object where 15 is the MovieCount
        var dataFactory = new DataFactory(15);

        // app.MapGet("/") defines an endpoint that responds to GET requests made to the root URL of the application (i.e., http://localhost:<port>/)
        // lambda expression that serves as the handler for the GET request - returns a result using Results.Ok(...), 
        // which is a method that creates a response with an HTTP status code of 200 (OK) and includes a specified message as the response body
        app.MapGet("/", () => Results.Ok("Welcome to our API!"));

        // the #region directive is used to define a block of code that can be expanded or collapsed in the code editor, making it easier to organize 
        // and navigate through larger files
        #region Movies

        // Endpoint to fetch the list of all movies
        apiGroup.MapGet("/movies", () => Results.Ok(dataFactory.Movies));

        // Endpoint to fetch a specific movie by its ID
        apiGroup.MapGet("/movie", (int id) =>
        {
            // Get the movie by its ID from the DataFactory
            var movie = dataFactory.GetMovieById(id);

            // If movie is not found, return a 404 Not Found response
            if (movie == null)
            {
                return Results.NotFound();
            }

            // Return the movie object with a 200 OK response
            return Results.Ok(movie);
        });

        // Endpoint to create a new movie entry
        apiGroup.MapPost("/movies", (string title, string description, double totalBudget, double totalCost, DateTime releaseDate, List<Guid> crewIds) =>
        {
            try
            {
                // Create a new movie using the provided parameters
                var created = dataFactory.CreateMovie(title, description, totalBudget, totalCost, releaseDate, crewIds);

                // Return a 201 Created response, including the location of the newly created movie
                return Results.Created($"api/movie?id={created.Id}", created);
            }
            catch (Exception ex)
            {
                // If there's an exception (e.g., validation error), return a 400 Bad Request response
                return Results.BadRequest(ex.Message);
            }
        });

        #endregion

        #region Persons

        apiGroup.MapGet("/persons", () => Results.Ok(dataFactory.Persons));

        apiGroup.MapGet("/person", (Guid id) =>
        {
            var person = dataFactory.GetPersonById(id);

            if (person == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(person);
        });

        // Endpoint to create a new person entry
        apiGroup.MapPost("/persons", (string name, DateTime birthDate, int mostFamousMovie) =>
        {
            try
            {
                var created = dataFactory.CreatePerson(name, birthDate, mostFamousMovie);

                return Results.Created($"api/person?id={created.Id}", created);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });

        #endregion

        // Run the application and start listening for HTTP requests
        app.Run();
    }
}
