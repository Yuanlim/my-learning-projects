using System.ComponentModel.DataAnnotations;
namespace MyNewApp.Dtos;

public record class CreateGameDto
(
    [Required][StringLength(50)] string Name,
    int GenreId,
    [Required][Range(1, 300)]decimal Price,
    DateOnly ReleaseDate // dont care about time 
);
