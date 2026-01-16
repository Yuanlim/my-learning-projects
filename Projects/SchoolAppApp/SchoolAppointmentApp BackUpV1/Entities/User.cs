using System;

namespace SchoolAppointmentApp.Entities;

// One class of user e.g.(Student/Teacher/.. Points to One User class)
public class User
{
    public int UserId { set; get; }
    public required string Name { set; get; }
    public string? PhoneNumber { set; get; }
    public string? Email { set; get; }
    public string? PasswordHash { set; get; }
    public bool Terminate { set; get; } = false;
    public Teacher? Teacher { set; get; }
    public Student? Student { set; get; }
}
