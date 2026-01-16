namespace MyNewApp.Dtos;

public record class GameSummaryDto(
    int Id,
    string Name,
    string Genre,
    decimal Price,
    DateOnly ReleaseDate // dont care about time 
);

public record class GameDetailsDto(
    int Id,
    string Name,
    int GenreId,
    decimal Price,
    DateOnly ReleaseDate // dont care about time 
);

public record Todo(
    int Id,
    string Name,
    DateTime DueDate,
    bool IsCompleted
);
