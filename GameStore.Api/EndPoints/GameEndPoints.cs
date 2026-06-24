using System.Net.NetworkInformation;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.EndPoints;

public static class GameEndPoints
{
    public static void MapGamesEndPoints(this WebApplication app)
    {

        var group = app.MapGroup("/games");


        // find all
        group.MapGet("/", async (GameStoreContext dbCtx) => await dbCtx.Games.Select(game => new GameDto(
        game.Id,
        game.Name,
        game.Genre!.Name,
        game.Price,
        game.Date
        )).ToListAsync());

        // find one
        group.MapGet("/{id}", async (int Id, GameStoreContext dbCtx) =>
        {
            var game = await dbCtx.Games.Where(game => game.Id.Equals(Id)).Select(game => new GameDto(
            game.Id,
            game.Name,
            game.Genre!.Name,
            game.Price,
            game.Date
            )).FirstOrDefaultAsync();

            if (game is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(game);

        }).WithName("GetSingleGameName");


        // post

        group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbCtx) =>
        {
            Game game = new()
            {
                Name = newGame.Name,
                GenreId = newGame.GenreId,
                Price = newGame.Price,
                Date = newGame.Date,
            };

            dbCtx.Games.Add(game);
            dbCtx.SaveChanges();

            GameDetailsDto gameDto = new(
                game.Id,
                game.Name,
                game.GenreId,
                game.Price,
                game.Date
            );



            return Results.CreatedAtRoute("GetSingleGameName", new { id = gameDto.Id }, gameDto);

        });


        // put

        group.MapPut("/{id}", async (int Id, CreateGameDto newGame, GameStoreContext dbCtx) =>
        {

            var game = await dbCtx.Games.FindAsync(Id);

            if (game is null)
            {
                return Results.NotFound();
            }

            game.Name = newGame.Name;
            game.GenreId = newGame.GenreId;
            game.Price = newGame.Price;
            game.Date = newGame.Date;

            await dbCtx.SaveChangesAsync();

            return Results.NoContent();


        });


        // delete 
        group.MapDelete("/{Id}", async (int Id, GameStoreContext dbCtx) =>
        {

            var result = await dbCtx.Games.Where(g => g.Id == Id).ExecuteDeleteAsync();


            return result == 0 ? Results.NotFound() : Results.NoContent();

        });

    }

}