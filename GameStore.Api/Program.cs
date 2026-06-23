using GameStore.Api.Dtos;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


List<GameDto> games = [
    new (
        1,
        "Eshan",
        "Eshan",
        12.33M,
        new DateOnly(2006, 12, 1)
    ),
    new (
        2,
        "Penatly shotter",
        "Action",
        12,
        new DateOnly(2000, 12, 3)
    )
];

// find all
app.MapGet("/", () => games);

// find one
app.MapGet("/{id}", (int Id) =>
{
    var game = games.Find(game => game.Id == Id);
    if (game is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(game);

}).WithName("GetSingleGameName");


// post

app.MapPost("/", (CreateGameDto newGame) =>
{
    GameDto game = new(
        games.Count + 1,
        newGame.Name,
        newGame.Genre,
        newGame.Price,
        newGame.Date
    );

    games.Add(game);

    return Results.CreatedAtRoute("GetSingleGameName", new { id = game.Id }, game);

});


// put

app.MapPut("/{id}", (int Id, CreateGameDto newGame) =>
{
    var index = games.FindIndex(game => game.Id == Id);

    games[index] = new(
        Id,
        newGame.Name,
        newGame.Genre,
        newGame.Price,
        newGame.Date
    );

    return Results.NoContent();


});


// delete 
app.MapDelete("/{Id}", (int Id) =>
{

    var index = games.FindIndex(game => game.Id == Id);

    games.RemoveAt(index);

    return Results.NoContent();

});

app.Run();
