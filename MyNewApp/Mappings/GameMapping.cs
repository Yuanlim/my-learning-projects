using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using MyNewApp.Data;
using MyNewApp.Dtos;
using MyNewApp.Entities;

namespace MyNewApp.Mappings;

public static class GameMapping
{
    /// <summary>
    /// Request translate to table field 
    /// </summary>
    /// <returns>Game table object</returns>
    public static Game ToEntity(this CreateGameDto newGame) // extension of CreateGameDto
    {
        return new Game
        {
            Name = newGame.Name,
            GenreId = newGame.GenreId,
            Price = newGame.Price,
            ReleaseDate = newGame.ReleaseDate
        };
    }

    public static Game ToEntity(this UpdateGameDto newGame, int id) // extension of CreateGameDto
    {
        return new Game
        {
            Id = id,
            Name = newGame.Name,
            GenreId = newGame.GenreId,
            Price = newGame.Price,
            ReleaseDate = newGame.ReleaseDate
        };
    }

    public static Game ToEntity(
        this PatchGameRecords newGame,
        Game targetGame, int id)
    {
        return new ()
        {
            Id = id,
            Name = newGame.Name ?? targetGame.Name,
            GenreId = newGame.GenreId ?? targetGame.GenreId,
            Price = newGame.Price ?? targetGame.Price,
            ReleaseDate = newGame.ReleaseDate ?? targetGame.ReleaseDate
        };  
    } 
            
    public static GameSummaryDto ToSummaryDto(this Game newGame)
    {
        return new GameSummaryDto(
            newGame.Id,
            newGame.Name,
            newGame.Genre!.Name, // !: it "should" not be null here
            newGame.Price,
            newGame.ReleaseDate
        );
    }

    public static GameDetailsDto ToDetailsDto(this Game game)
    {
        return new GameDetailsDto(
            game.Id,
            game.Name,
            game.GenreId, // !: it "should" not be null here
            game.Price,
            game.ReleaseDate
        );
    }

}
