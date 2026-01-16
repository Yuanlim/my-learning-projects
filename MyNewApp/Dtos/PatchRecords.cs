namespace MyNewApp.Dtos;

public record class PatchGameRecords (
    string? Name,
    int? GenreId,
    decimal? Price,
    DateOnly? ReleaseDate
);