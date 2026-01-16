namespace SchoolAppointmentApp.Entities;

public class SchoolPrincipal
{
    public int Id { get; set; }
    public required string PrincipalId { get; set; }
    public required string PasswordHash { get; set; }
    public string? Email { get; set; }
    public string? Contact { get; set; }
}
