using GameStore.Api.Data;
using GameStore.Api.dtos;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.EndPoints;

public static class GenreEndPoints
{

    public static void MapGenreEndPoints(this WebApplication app)
    {

        var group = app.MapGroup("/genre");

        // get all genre
        group.MapGet("/", async (GameStoreContext dbCtx) =>
        {
            var result = await dbCtx.Genres.ToListAsync();
            return Results.Ok(result);
        });

        group.MapGet("/{id}", async (int Id, GameStoreContext dbCtx) =>
        {
            var result = await dbCtx.Genres.FindAsync(Id);

            return result is null ? Results.NotFound() : Results.Ok(result);

        }).WithName("GetGenreById");


        // post genre
        group.MapPost("/", async (GenreDto newGenre, GameStoreContext dbCtx) =>
        {
            Genre genre = new()
            {
                Id = newGenre.Id,
                Name = newGenre.Name
            };

            await dbCtx.Genres.AddAsync(genre);
            await dbCtx.SaveChangesAsync();

            var genreDto = new GenreDto(
            genre.Id,
            genre.Name
            );

            return Results.CreatedAtRoute("GetGenreById", new { id = genreDto.Id }, genreDto);

        });


    }




}
