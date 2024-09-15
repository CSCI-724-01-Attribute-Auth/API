using API;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var apiGroup = app.MapGroup("/api");

var dataFactory = new DataFactory(15);

app.MapGet("/", () => Results.Ok("Welcome to our API!"));

#region Movies

apiGroup.MapGet("/movies", () => Results.Ok(dataFactory.Movies));

apiGroup.MapGet("/movie", (int id) =>
{
    var movie = dataFactory.GetMovieById(id);

    if (movie == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(movie);
});

apiGroup.MapPost("/movies", (string title, string description, double totalBudget, double totalCost, DateTime releaseDate, List<Guid> crewIds) =>
{
    try
    {
        var created = dataFactory.CreateMovie(title, description, totalBudget, totalCost, releaseDate, crewIds);

        return Results.Created($"api/movie?id={created.Id}", created);
    }
    catch (Exception ex)
    {
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

app.Run();
