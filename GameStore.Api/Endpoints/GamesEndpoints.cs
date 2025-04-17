using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    private const string GetGameEndpointName = "GetGame";

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();

        //Get all games
        // refactor to asyncronous method
        group.MapGet("/", async (GameStoreContext dbContext) => await dbContext.Games
        .Include(game => game.Genre)
        .Select(game => game.ToGameSummaryDto())
        .AsNoTracking()
        .ToListAsync());

        //Get game by id
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) => 
        {
            Game? game = await dbContext.Games.FindAsync(id);

            return game is null ? Results.NotFound() : Results.Ok(game.ToGameDetaisDto());
        }).WithName(GetGameEndpointName);

        //Create a new game
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = newGame.ToEntity();

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();
   
            return Results.CreatedAtRoute(
                GetGameEndpointName,
                new { id = game.Id },
                 game.ToGameDetaisDto());
        });

        //Update an existing game
        group.MapPut("/{id}", async (int id, UpdateGameDto updateGame, GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id);

            if (existingGame is null)
            {
                return Results.NotFound();
            }

            //locate the existing entry in the context
            dbContext.Entry(existingGame).CurrentValues.SetValues(updateGame.ToEntity(id));
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        app.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
          await dbContext.Games.Where(game => game.Id == id).ExecuteDeleteAsync();
            return Results.NoContent();
        });

        return group;
    } 
}