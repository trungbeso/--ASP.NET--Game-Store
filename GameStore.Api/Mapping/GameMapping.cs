using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Mapping;

public static class GameMapping {
  public static Game ToEntity (this CreateGameDto gameDto) {
     return new () {
        Name = gameDto.Name,
        GenreId = gameDto.GenreId,
        Price = gameDto.Price,
        ReleaseDate = gameDto.ReleaseDate
      };    
  }

  public static GameDto ToDto (this Game game) {
    return new (
      game.Id,
      game.Name,
      game.Genre!.Name,
      game.Price,
      game.ReleaseDate
    );
  }
}
              