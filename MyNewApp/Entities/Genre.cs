namespace MyNewApp.Entities;
/// <summary>
/// Create table name Genre
/// Id a primary key corresponds to one Type
/// </summary>
public class Genre
{
    public int Id { get; set; }

    public required string Name { set; get; }

}
