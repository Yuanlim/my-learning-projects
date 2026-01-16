namespace SchoolAppointmentApp.DataTypeObject;

public record class StudentDto(
    string StudentId,
    string Name,
    string ClassName,
    string PhoneNumber,
    string Email
);

public record class TeacherDto(
    string TeacherId,
    string Name,
    string PhoneNumber,
    string Email,
    int Points = 0
);

