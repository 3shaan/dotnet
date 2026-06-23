using GameStore.Api.Dtos;

namespace GameStore.Api.EndPoints;

public static class GameEndPoints
{

    static private readonly List<GameDto> games = [
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


    public static void MapGamesEndPoints(this WebApplication app)
    {

        var group = app.MapGroup("/games");


        // find all
        group.MapGet("/", () => games);

        // find one
        group.MapGet("/{id}", (int Id) =>
        {
            var game = games.Find(game => game.Id == Id);
            if (game is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(game);

        }).WithName("GetSingleGameName");


        // post

        group.MapPost("/", (CreateGameDto newGame) =>
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

        group.MapPut("/{id}", (int Id, CreateGameDto newGame) =>
        {
            var index = games.FindIndex(game => game.Id == Id);

            if (index is -1)
            {
                return Results.NotFound();
            }

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
        group.MapDelete("/{Id}", (int Id) =>
        {

            var index = games.FindIndex(game => game.Id == Id);

            if (index is -1)
            {
                return Results.NotFound();
            }

            games.RemoveAt(index);

            return Results.NoContent();

        });

    }

}