using GameStore.Api.Data;
using GameStore.Api.EndPoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();

builder.AddGameStoreDb();

var app = builder.Build();


app.MapGamesEndPoints();

// migrate database
app.MigrateDb();


app.Run();
