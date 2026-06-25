using GameStore.Api.Data;
using GameStore.Api.EndPoints;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();

builder.AddGameStoreDb();

builder.Services.AddOpenApi();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}


//game
app.MapGamesEndPoints();

// genre
app.MapGenreEndPoints();

// migrate database
app.MigrateDb();


app.Run();
