using System.ComponentModel.DataAnnotations;

namespace MyNewApp.Dtos;

public record class UpdateGameDto(
    // When user request PUT, input can have null.
    // use [Required] attr -> user must input
    // StringLength -> max length input
    // range -> values range
    // Need to + nuget parameter validation inorder to check
    [Required][StringLength(50)] string Name,
    int GenreId,
    [Required][Range(1, 200)] decimal Price,
    DateOnly ReleaseDate
);

