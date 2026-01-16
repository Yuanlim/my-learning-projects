using System;

namespace SchoolAppointmentApp.Entities;

public class Teacher
{
    public int UserId { set; get; }
    public string TeacherId { get; set; } = null!;
    public int Points { set; get; }
    public int TodaysEarning { set; get; }
    public User? User { get; set; }
}

