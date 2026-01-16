using System;

namespace MyNewApp.Entities;

/// <summary>
/// Create Game table
/// GenreId points to 1 type of genre name 
/// </summary>
public class Game
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int GenreId { get; set; }
    public Genre? Genre { get; set; }
    public decimal Price { get; set; }
    public DateOnly ReleaseDate { get; set; }
}

