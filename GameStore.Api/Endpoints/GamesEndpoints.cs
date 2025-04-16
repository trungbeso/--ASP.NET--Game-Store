using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    private const string GetGameEndpointName = "GetGame";

    private static readonly List<GameDto> Games = [
        new (1, "The Legend of Zelda: Breath of the Wild", "Action-adventure", 59.99m, new DateOnly(2017, 3, 3)),
        new (2, "Super Mario Odyssey", "Platformer", 59.99m, new DateOnly(2017, 10, 27)),
        new (3, "The Witcher 3: Wild Hunt", "Action RPG", 39.99m, new DateOnly(2015, 5, 19)),
        new (4, "Dark Souls III", "Action RPG", 39.99m, new DateOnly(2016, 3, 24)),
        new (5, "Hollow Knight", "Metroidvania", 14.99m, new DateOnly(2017, 2, 24))
    ];

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();

        //Get all games
        group.MapGet("/", () => Games);

        //Get game by id
        group.MapGet("/{id}", (int id, GameStoreContext dbContext) => 
        {
            Game? game = dbContext.Games.Find(id);

            return game is null ? Results.NotFound() : Results.Ok(game);
        }).WithName(GetGameEndpointName);

        //Create a new game
        group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = GameMapping.ToEntity(newGame);
            game.Genre = dbContext.Genres.Find(newGame.GenreId);

            dbContext.Games.Add(game);
            dbContext.SaveChanges();

            return Results.CreatedAtRoute(
                GetGameEndpointName, 
                new { id = game }, game.ToDto());
        });

        //Update an existing game
        group.MapPut("/{id}", (int id, UpdateGameDto updateGame) =>
        {
            var index = Games.FindIndex(game => game.Id == id);

            if (index == -1)
            {
                return Results.NotFound();
            }

            Games[index] = new GameDto(
                id,
                updateGame.Name,
                updateGame.Genre,
                updateGame.Price,
                updateGame.ReleaseDate
            );

            return Results.NoContent();
        });

        app.MapDelete("/{id}", (int id) =>
        {
            Games.RemoveAll(game => game.Id == id);
            return Results.NoContent();
        });

        return group;
    } 
}