using SchoolAppointmentApp.Entities;

// Not link with users cause you cant block, friend or report Admin
public class Admin
{
    public int AdminId { get; set; }
    public required string AdminLoginId { get; set; }
    public required string PasswordHash { get; set; }
    public string? Email { get; set; }
    public string? Contact { get; set; }
}