namespace SchoolAppointmentApp.Entities;

public class Student
{
    public string StudentId { set; get; } = null!;
    public int ClassId { set; get; }
    public SchoolClass SchoolClass { set; get; } = null!;
    public int UserId { set; get; } // FK.keys of User.Id
    public User? User { get; set; }
}


