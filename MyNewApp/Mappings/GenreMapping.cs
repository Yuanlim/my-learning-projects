using System;
using MyNewApp.Dtos;
using MyNewApp.Entities;

namespace MyNewApp.Mappings;

public static class GenreMapping
{
    public static GenreDto ToDto(this Genre genre)
    {
        return new GenreDto(genre.Id, genre.Name);
    } 
}
