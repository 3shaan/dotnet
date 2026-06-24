using GameStore.Api.Data;
using GameStore.Api.EndPoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();

builder.AddGameStoreDb();

var app = builder.Build();


//game
app.MapGamesEndPoints();

// genre
app.MapGenreEndPoints();

// migrate database
app.MigrateDb();


app.Run();
